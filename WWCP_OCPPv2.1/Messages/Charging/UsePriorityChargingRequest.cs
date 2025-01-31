﻿/*
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
    /// The UsePriorityCharging request.
    /// </summary>
    public class UsePriorityChargingRequest : ARequest<UsePriorityChargingRequest>,
                                              IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/usePriorityChargingRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The transaction for which priority charging is requested.
        /// </summary>
        [Mandatory]
        public Transaction_Id  TransactionId    { get; }

        /// <summary>
        /// True, to request priority charging,
        /// or false, when to request stopping priority charging.
        /// </summary>
        [Mandatory]
        public Boolean         Activate         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an UsePriorityCharging request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="TransactionId">The transaction for which priority charging is requested.</param>
        /// <param name="Activate">True, when priority charging was activated, or false, when it has stopped using the priority charging profile.</param>
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
        public UsePriorityChargingRequest(SourceRouting            Destination,
                                          Transaction_Id           TransactionId,
                                          Boolean                  Activate,

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
                   nameof(UsePriorityChargingRequest)[..^7],

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

            this.TransactionId  = TransactionId;
            this.Activate       = Activate;

            unchecked
            {
                hashCode = TransactionId.GetHashCode() * 5 ^
                           Activate.     GetHashCode() * 3 ^
                           base.         GetHashCode();
            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomUsePriorityChargingRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an UsePriorityCharging request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomUsePriorityChargingRequestParser">A delegate to parse custom UsePriorityCharging requests.</param>
        public static UsePriorityChargingRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       SourceRouting                                         Destination,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 RequestTimestamp                         = null,
                                                       TimeSpan?                                                 RequestTimeout                           = null,
                                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                                       CustomJObjectParserDelegate<UsePriorityChargingRequest>?  CustomUsePriorityChargingRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var usePriorityChargingRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomUsePriorityChargingRequestParser))
            {
                return usePriorityChargingRequest;
            }

            throw new ArgumentException("The given JSON representation of an UsePriorityCharging request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out UsePriorityChargingRequest, out ErrorResponse, CustomUsePriorityChargingRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an UsePriorityCharging request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="UsePriorityChargingRequest">The parsed UsePriorityCharging request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomUsePriorityChargingRequestParser">A delegate to parse custom UsePriorityCharging requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out UsePriorityChargingRequest?      UsePriorityChargingRequest,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 RequestTimestamp                         = null,
                                       TimeSpan?                                                 RequestTimeout                           = null,
                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                       CustomJObjectParserDelegate<UsePriorityChargingRequest>?  CustomUsePriorityChargingRequestParser   = null)
        {

            try
            {

                UsePriorityChargingRequest = null;

                #region TransactionId        [mandatory]

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id TransactionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Activate             [mandatory]

                if (!JSON.ParseMandatory("activate",
                                         "activate",
                                         out Boolean Activate,
                                         out ErrorResponse))
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
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                UsePriorityChargingRequest = new UsePriorityChargingRequest(

                                                 Destination,
                                                 TransactionId,
                                                 Activate,

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

                if (CustomUsePriorityChargingRequestParser is not null)
                    UsePriorityChargingRequest = CustomUsePriorityChargingRequestParser(JSON,
                                                                                        UsePriorityChargingRequest);

                return true;

            }
            catch (Exception e)
            {
                UsePriorityChargingRequest  = null;
                ErrorResponse               = "The given JSON representation of an UsePriorityCharging request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUsePriorityChargingRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUsePriorityChargingRequestSerializer">A delegate to serialize custom UsePriorityCharging requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                       IncludeJSONLDContext                         = false,
                              CustomJObjectSerializerDelegate<UsePriorityChargingRequest>?  CustomUsePriorityChargingRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",        DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("transactionId",   TransactionId.       ToString()),
                                 new JProperty("activate",        Activate),

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomUsePriorityChargingRequestSerializer is not null
                       ? CustomUsePriorityChargingRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UsePriorityChargingRequest1, UsePriorityChargingRequest2)

        /// <summary>
        /// Compares two UsePriorityCharging requests for equality.
        /// </summary>
        /// <param name="UsePriorityChargingRequest1">A UsePriorityCharging request.</param>
        /// <param name="UsePriorityChargingRequest2">Another UsePriorityCharging request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UsePriorityChargingRequest? UsePriorityChargingRequest1,
                                           UsePriorityChargingRequest? UsePriorityChargingRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UsePriorityChargingRequest1, UsePriorityChargingRequest2))
                return true;

            // If one is null, but not both, return false.
            if (UsePriorityChargingRequest1 is null || UsePriorityChargingRequest2 is null)
                return false;

            return UsePriorityChargingRequest1.Equals(UsePriorityChargingRequest2);

        }

        #endregion

        #region Operator != (UsePriorityChargingRequest1, UsePriorityChargingRequest2)

        /// <summary>
        /// Compares two UsePriorityCharging requests for inequality.
        /// </summary>
        /// <param name="UsePriorityChargingRequest1">A UsePriorityCharging request.</param>
        /// <param name="UsePriorityChargingRequest2">Another UsePriorityCharging request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UsePriorityChargingRequest? UsePriorityChargingRequest1,
                                           UsePriorityChargingRequest? UsePriorityChargingRequest2)

            => !(UsePriorityChargingRequest1 == UsePriorityChargingRequest2);

        #endregion

        #endregion

        #region IEquatable<UsePriorityChargingRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UsePriorityCharging requests for equality.
        /// </summary>
        /// <param name="Object">A UsePriorityCharging request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UsePriorityChargingRequest usePriorityChargingRequest &&
                   Equals(usePriorityChargingRequest);

        #endregion

        #region Equals(UsePriorityChargingRequest)

        /// <summary>
        /// Compares two UsePriorityCharging requests for equality.
        /// </summary>
        /// <param name="UsePriorityChargingRequest">A UsePriorityCharging request to compare with.</param>
        public override Boolean Equals(UsePriorityChargingRequest? UsePriorityChargingRequest)

            => UsePriorityChargingRequest is not null &&

               TransactionId.Equals(UsePriorityChargingRequest.TransactionId) &&
               Activate.     Equals(UsePriorityChargingRequest.Activate)      &&

               base.GenericEquals(UsePriorityChargingRequest);

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

            => $"'{TransactionId}' shall be {(Activate ? "activated" : "deactivated")}";

        #endregion

    }

}
