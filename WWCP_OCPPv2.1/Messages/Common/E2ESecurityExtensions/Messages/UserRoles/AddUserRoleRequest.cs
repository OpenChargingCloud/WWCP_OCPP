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

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The AddUserRole request.
    /// </summary>
    public class AddUserRoleRequest : ARequest<AddUserRoleRequest>,
                                      IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/addUserRoleRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The user role.
        /// </summary>
        [Mandatory]
        public UserRole       UserRole    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new AddUserRole request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="UserRole">A user role.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public AddUserRoleRequest(SourceRouting            Destination,
                                  UserRole                 UserRole,

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
                   nameof(AddUserRoleRequest)[..^7],

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

            this.UserRole = UserRole;

            unchecked
            {
                hashCode = this.UserRole.GetHashCode() * 3 ^
                           base.         GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomAddUserRoleRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an AddUserRole request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomAddUserRoleRequestParser">An optional delegate to parse custom AddUserRole requests.</param>
        public static AddUserRoleRequest Parse(JObject                                           JSON,
                                               Request_Id                                        RequestId,
                                               SourceRouting                                 Destination,
                                               NetworkPath                                       NetworkPath,
                                               DateTime?                                         RequestTimestamp                 = null,
                                               TimeSpan?                                         RequestTimeout                   = null,
                                               EventTracking_Id?                                 EventTrackingId                  = null,
                                               CustomJObjectParserDelegate<AddUserRoleRequest>?  CustomAddUserRoleRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var addUserRoleRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomAddUserRoleRequestParser))
            {
                return addUserRoleRequest;
            }

            throw new ArgumentException("The given JSON representation of an AddUserRole request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out AddUserRoleRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an AddUserRole request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="AddUserRoleRequest">The parsed AddUserRole request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomAddUserRoleRequestParser">An optional delegate to parse custom AddUserRole requests.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       Request_Id                                        RequestId,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out AddUserRoleRequest?      AddUserRoleRequest,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTime?                                         RequestTimestamp                 = null,
                                       TimeSpan?                                         RequestTimeout                   = null,
                                       EventTracking_Id?                                 EventTrackingId                  = null,
                                       CustomJObjectParserDelegate<AddUserRoleRequest>?  CustomAddUserRoleRequestParser   = null)
        {

            try
            {

                AddUserRoleRequest = null;

                #region UserRole      [mandatory]

                if (!JSON.ParseMandatoryJSON("signaturePolicy",
                                             "user role",
                                             WWCP.UserRole.TryParse,
                                             out UserRole? UserRole,
                                             out ErrorResponse) ||
                     UserRole is null)
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                AddUserRoleRequest = new AddUserRoleRequest(

                                         Destination,
                                         UserRole,

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

                if (CustomAddUserRoleRequestParser is not null)
                    AddUserRoleRequest = CustomAddUserRoleRequestParser(JSON,
                                                                        AddUserRoleRequest);

                return true;

            }
            catch (Exception e)
            {
                AddUserRoleRequest  = null;
                ErrorResponse       = "The given JSON representation of an AddUserRole request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAddUserRoleRequestSerializer = null, CustomUserRoleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAddUserRoleRequestSerializer">A delegate to serialize custom AddUserRole requests.</param>
        /// <param name="CustomUserRoleSerializer">A delegate to serialize custom signature policies.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AddUserRoleRequest>?  CustomAddUserRoleRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<UserRole>?            CustomUserRoleSerializer             = null,
                              CustomJObjectSerializerDelegate<Signature>?           CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("signaturePolicy",   UserRole.ToJSON()),

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAddUserRoleRequestSerializer is not null
                       ? CustomAddUserRoleRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AddUserRoleRequest1, AddUserRoleRequest2)

        /// <summary>
        /// Compares two AddUserRole requests for equality.
        /// </summary>
        /// <param name="AddUserRoleRequest1">An AddUserRole request.</param>
        /// <param name="AddUserRoleRequest2">Another AddUserRole request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AddUserRoleRequest? AddUserRoleRequest1,
                                           AddUserRoleRequest? AddUserRoleRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AddUserRoleRequest1, AddUserRoleRequest2))
                return true;

            // If one is null, but not both, return false.
            if (AddUserRoleRequest1 is null || AddUserRoleRequest2 is null)
                return false;

            return AddUserRoleRequest1.Equals(AddUserRoleRequest2);

        }

        #endregion

        #region Operator != (AddUserRoleRequest1, AddUserRoleRequest2)

        /// <summary>
        /// Compares two AddUserRole requests for inequality.
        /// </summary>
        /// <param name="AddUserRoleRequest1">An AddUserRole request.</param>
        /// <param name="AddUserRoleRequest2">Another AddUserRole request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AddUserRoleRequest? AddUserRoleRequest1,
                                           AddUserRoleRequest? AddUserRoleRequest2)

            => !(AddUserRoleRequest1 == AddUserRoleRequest2);

        #endregion

        #endregion

        #region IEquatable<AddUserRoleRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AddUserRole requests for equality.
        /// </summary>
        /// <param name="Object">An AddUserRole request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AddUserRoleRequest addUserRoleRequest &&
                   Equals(addUserRoleRequest);

        #endregion

        #region Equals(AddUserRoleRequest)

        /// <summary>
        /// Compares two AddUserRole requests for equality.
        /// </summary>
        /// <param name="AddUserRoleRequest">An AddUserRole request to compare with.</param>
        public override Boolean Equals(AddUserRoleRequest? AddUserRoleRequest)

            => AddUserRoleRequest is not null &&

               UserRole.Equals(AddUserRoleRequest.UserRole) &&

               base.    GenericEquals(AddUserRoleRequest);

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

            => UserRole.ToString();

        #endregion

    }

}
