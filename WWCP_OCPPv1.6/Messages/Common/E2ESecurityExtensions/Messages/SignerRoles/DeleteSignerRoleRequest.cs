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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The DeleteSignerRole request: take the keys of a signer role away.
    /// </summary>
    /// <remarks>
    /// White paper "Signed OCPP Messages and Signer Roles", section 6.4: the
    /// message of OCPP 2.1, as an action of OCPP 1.6.
    /// </remarks>
    public class DeleteSignerRoleRequest : ARequest<DeleteSignerRoleRequest>,
                                          IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/deleteSignerRoleRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The role whose keys are taken away.
        /// </summary>
        [Mandatory]
        public String              Role                 { get; }

        /// <summary>
        /// The version of the role assignment that is to be deleted: a delete of another one came too late.
        /// </summary>
        [Mandatory]
        public UInt64              Version              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DeleteSignerRole request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Role">The role whose keys are taken away.</param>
        /// <param name="Version">The version of the role assignment that is to be deleted.</param>
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
        public DeleteSignerRoleRequest(SourceRouting            Destination,
                                      String                   Role,
                                      UInt64                   Version,

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
                   nameof(DeleteSignerRoleRequest)[..^7],

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

            this.Role     = Role;
            this.Version  = Version;

            unchecked
            {
                hashCode = (this.Role.GetHashCode() * 5 ^ this.Version.GetHashCode()) * 3 ^
                           base.GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a DeleteSignerRole request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomDeleteSignerRoleRequestParser">An optional delegate to parse custom DeleteSignerRole requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static DeleteSignerRoleRequest Parse(JObject                                               JSON,
                                                   Request_Id                                            RequestId,
                                                   SourceRouting                                         Destination,
                                                   NetworkPath                                           NetworkPath,
                                                   DateTimeOffset?                                       RequestTimestamp                     = null,
                                                   TimeSpan?                                             RequestTimeout                       = null,
                                                   EventTracking_Id?                                     EventTrackingId                      = null,
                                                   CustomJObjectParserDelegate<DeleteSignerRoleRequest>?  CustomDeleteSignerRoleRequestParser   = null,
                                                   CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                                   CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var deleteSignerRoleRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomDeleteSignerRoleRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return deleteSignerRoleRequest;
            }

            throw new ArgumentException("The given JSON representation of a DeleteSignerRole request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out DeleteSignerRoleRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a DeleteSignerRole request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="DeleteSignerRoleRequest">The parsed DeleteSignerRole request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDeleteSignerRoleRequestParser">An optional delegate to parse custom DeleteSignerRole requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out DeleteSignerRoleRequest?      DeleteSignerRoleRequest,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       DateTimeOffset?                                       RequestTimestamp                     = null,
                                       TimeSpan?                                             RequestTimeout                       = null,
                                       EventTracking_Id?                                     EventTrackingId                      = null,
                                       CustomJObjectParserDelegate<DeleteSignerRoleRequest>?  CustomDeleteSignerRoleRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                       CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {

            try
            {

                DeleteSignerRoleRequest = null;

                #region Role                 [mandatory]

                if (JSON["role"]?.Type != JTokenType.String ||
                    JSON["role"]!.Value<String>() is not String Role ||
                    Role.Length == 0)
                {
                    ErrorResponse = "The role is missing!";
                    return false;
                }

                #endregion

                #region Version              [mandatory]

                if (JSON["version"]?.Type != JTokenType.Integer ||
                    !UInt64.TryParse(JSON["version"]!.ToString(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var Version))
                {
                    ErrorResponse = "The version of the role assignment is missing, or is not a number that counts upwards!";
                    return false;
                }

                #endregion

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                DeleteSignerRoleRequest = new DeleteSignerRoleRequest(

                                             Destination,
                                             Role,
                                             Version,

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

                if (CustomDeleteSignerRoleRequestParser is not null)
                    DeleteSignerRoleRequest = CustomDeleteSignerRoleRequestParser(JSON,
                                                                                DeleteSignerRoleRequest);

                return true;

            }
            catch (Exception e)
            {
                DeleteSignerRoleRequest  = null;
                ErrorResponse           = "The given JSON representation of a DeleteSignerRole request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDeleteSignerRoleRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteSignerRoleRequestSerializer">A delegate to serialize custom DeleteSignerRole requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteSignerRoleRequest>?  CustomDeleteSignerRoleRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?               CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("role",                Role),
                                 new JProperty("version",             Version),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDeleteSignerRoleRequestSerializer is not null
                       ? CustomDeleteSignerRoleRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out DeleteSignerRoleRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a DeleteSignerRole request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="DeleteSignerRoleRequest">The DeleteSignerRole request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomDeleteSignerRoleRequestParser">A delegate to read custom DeleteSignerRole requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                          CBOR,
                                           Request_Id                                         RequestId,
                                           SourceRouting                                      Destination,
                                           NetworkPath                                        NetworkPath,
                                           [NotNullWhen(true)]  out DeleteSignerRoleRequest?   DeleteSignerRoleRequest,
                                           [NotNullWhen(false)] out String?                   ErrorResponse,
                                           DateTimeOffset?                                    RequestTimestamp                     = null,
                                           TimeSpan?                                          RequestTimeout                       = null,
                                           EventTracking_Id?                                  EventTrackingId                      = null,
                                           CustomCBORParserDelegate<DeleteSignerRoleRequest>?  CustomDeleteSignerRoleRequestParser   = null)
        {

            try
            {

                DeleteSignerRoleRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a DeleteSignerRole request is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("role",
                                             "role",
                                             out var Role,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatory("version",
                                         "version of the role assignment",
                                         SignedObjectCBORExtensions.TryParseUInt64,
                                         out UInt64 Version,
                                         out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptionalList<Signature>("signatures",
                                               "cryptographic signatures",
                                               OCPPCBORExtensions.TryParseSignature,
                                               out var signatures,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? customData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                DeleteSignerRoleRequest = new DeleteSignerRoleRequest(

                                             Destination,
                                             Role,
                                             Version,

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

                if (CustomDeleteSignerRoleRequestParser is not null)
                    DeleteSignerRoleRequest = CustomDeleteSignerRoleRequestParser(CBOR,
                                                                               DeleteSignerRoleRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                DeleteSignerRoleRequest = null;
                ErrorResponse = "The given CBOR representation of a DeleteSignerRole request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomDeleteSignerRoleRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this DeleteSignerRole request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomDeleteSignerRoleRequestSerializer">A delegate to serialize custom DeleteSignerRole requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<DeleteSignerRoleRequest>? CustomDeleteSignerRoleRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("role",                      CBORValue.FromText(Role)),
                           ("version",                   CBORValue.FromUInt64(Version)),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomDeleteSignerRoleRequestSerializer is not null
                       ? CustomDeleteSignerRoleRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DeleteSignerRoleRequest1, DeleteSignerRoleRequest2)

        /// <summary>
        /// Compares two DeleteSignerRole requests for equality.
        /// </summary>
        /// <param name="DeleteSignerRoleRequest1">A DeleteSignerRole request.</param>
        /// <param name="DeleteSignerRoleRequest2">Another DeleteSignerRole request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DeleteSignerRoleRequest? DeleteSignerRoleRequest1,
                                           DeleteSignerRoleRequest? DeleteSignerRoleRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DeleteSignerRoleRequest1, DeleteSignerRoleRequest2))
                return true;

            // If one is null, but not both, return false.
            if (DeleteSignerRoleRequest1 is null || DeleteSignerRoleRequest2 is null)
                return false;

            return DeleteSignerRoleRequest1.Equals(DeleteSignerRoleRequest2);

        }

        #endregion

        #region Operator != (DeleteSignerRoleRequest1, DeleteSignerRoleRequest2)

        /// <summary>
        /// Compares two DeleteSignerRole requests for inequality.
        /// </summary>
        /// <param name="DeleteSignerRoleRequest1">A DeleteSignerRole request.</param>
        /// <param name="DeleteSignerRoleRequest2">Another DeleteSignerRole request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteSignerRoleRequest? DeleteSignerRoleRequest1,
                                           DeleteSignerRoleRequest? DeleteSignerRoleRequest2)

            => !(DeleteSignerRoleRequest1 == DeleteSignerRoleRequest2);

        #endregion

        #endregion

        #region IEquatable<DeleteSignerRoleRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DeleteSignerRole requests for equality.
        /// </summary>
        /// <param name="Object">A DeleteSignerRole request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteSignerRoleRequest deleteSignerRoleRequest &&
                   Equals(deleteSignerRoleRequest);

        #endregion

        #region Equals(DeleteSignerRoleRequest)

        /// <summary>
        /// Compares two DeleteSignerRole requests for equality.
        /// </summary>
        /// <param name="DeleteSignerRoleRequest">A DeleteSignerRole request to compare with.</param>
        public override Boolean Equals(DeleteSignerRoleRequest? DeleteSignerRoleRequest)

            => DeleteSignerRoleRequest is not null &&

               Role.   Equals(DeleteSignerRoleRequest.Role, StringComparison.Ordinal) &&
               Version.Equals(DeleteSignerRoleRequest.Version) &&

               base.GenericEquals(DeleteSignerRoleRequest);

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

            => $"Role: {Role}, version {Version}";

        #endregion

    }

}
