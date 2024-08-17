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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The NotifyPriorityCharging request.
    /// </summary>
    public class NotifyPriorityChargingRequest : ARequest<NotifyPriorityChargingRequest>,
                                                 IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/notifyPriorityChargingRequest");

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
        /// True, when priority charging was activated,
        /// or false, when it has stopped using the priority charging profile.
        /// </summary>
        [Mandatory]
        public Boolean         Activated        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a NotifyPriorityCharging request.
        /// </summary>
        /// <param name="SourceRouting">The destination networking node identification.</param>
        /// <param name="TransactionId">The transaction for which priority charging is requested.</param>
        /// <param name="Activated">True, when priority charging was activated, or false, when it has stopped using the priority charging profile.</param>
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
        public NotifyPriorityChargingRequest(SourceRouting            Destination,
                                             Transaction_Id           TransactionId,
                                             Boolean                  Activated,

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
                   nameof(NotifyPriorityChargingRequest)[..^7],

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
            this.Activated      = Activated;

            unchecked
            {
                hashCode = TransactionId.GetHashCode() * 5 ^
                           Activated.    GetHashCode() * 3 ^
                           base.         GetHashCode();
            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomNotifyPriorityChargingRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a NotifyPriorityCharging request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="SourceRouting">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyPriorityChargingRequestParser">A delegate to parse custom NotifyPriorityCharging requests.</param>
        public static NotifyPriorityChargingRequest Parse(JObject                                                      JSON,
                                                          Request_Id                                                   RequestId,
                                                          SourceRouting                                                SourceRouting,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    RequestTimestamp                            = null,
                                                          TimeSpan?                                                    RequestTimeout                              = null,
                                                          EventTracking_Id?                                            EventTrackingId                             = null,
                                                          CustomJObjectParserDelegate<NotifyPriorityChargingRequest>?  CustomNotifyPriorityChargingRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                             SourceRouting,
                         NetworkPath,
                         out var notifyPriorityChargingRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomNotifyPriorityChargingRequestParser))
            {
                return notifyPriorityChargingRequest;
            }

            throw new ArgumentException("The given JSON representation of a NotifyPriorityCharging request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out NotifyPriorityChargingRequest, out ErrorResponse, CustomNotifyPriorityChargingRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyPriorityCharging request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="SourceRouting">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyPriorityChargingRequest">The parsed NotifyPriorityCharging request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyPriorityChargingRequestParser">A delegate to parse custom NotifyPriorityCharging requests.</param>
        public static Boolean TryParse(JObject                                                      JSON,
                                       Request_Id                                                   RequestId,
                                       SourceRouting                                                SourceRouting,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out NotifyPriorityChargingRequest?      NotifyPriorityChargingRequest,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    RequestTimestamp                            = null,
                                       TimeSpan?                                                    RequestTimeout                              = null,
                                       EventTracking_Id?                                            EventTrackingId                             = null,
                                       CustomJObjectParserDelegate<NotifyPriorityChargingRequest>?  CustomNotifyPriorityChargingRequestParser   = null)
        {

            try
            {

                NotifyPriorityChargingRequest = null;

                #region TransactionId    [mandatory]

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id TransactionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Activated        [mandatory]

                if (!JSON.ParseMandatory("activated",
                                         "activated",
                                         out Boolean Activated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures       [optional, OCPP_CSE]

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

                #region CustomData       [optional]

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


                NotifyPriorityChargingRequest = new NotifyPriorityChargingRequest(

                                                        SourceRouting,
                                                    TransactionId,
                                                    Activated,

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

                if (CustomNotifyPriorityChargingRequestParser is not null)
                    NotifyPriorityChargingRequest = CustomNotifyPriorityChargingRequestParser(JSON,
                                                                                              NotifyPriorityChargingRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyPriorityChargingRequest  = null;
                ErrorResponse                  = "The given JSON representation of a NotifyPriorityCharging request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyPriorityChargingRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyPriorityChargingRequestSerializer">A delegate to serialize custom NotifyPriorityCharging requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyPriorityChargingRequest>?  CustomNotifyPriorityChargingRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("transactionId",   TransactionId.ToString()),
                                 new JProperty("activated",       Activated),

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.   ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyPriorityChargingRequestSerializer is not null
                       ? CustomNotifyPriorityChargingRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyPriorityChargingRequest1, NotifyPriorityChargingRequest2)

        /// <summary>
        /// Compares two NotifyPriorityCharging requests for equality.
        /// </summary>
        /// <param name="NotifyPriorityChargingRequest1">A NotifyPriorityCharging request.</param>
        /// <param name="NotifyPriorityChargingRequest2">Another NotifyPriorityCharging request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyPriorityChargingRequest? NotifyPriorityChargingRequest1,
                                           NotifyPriorityChargingRequest? NotifyPriorityChargingRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyPriorityChargingRequest1, NotifyPriorityChargingRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyPriorityChargingRequest1 is null || NotifyPriorityChargingRequest2 is null)
                return false;

            return NotifyPriorityChargingRequest1.Equals(NotifyPriorityChargingRequest2);

        }

        #endregion

        #region Operator != (NotifyPriorityChargingRequest1, NotifyPriorityChargingRequest2)

        /// <summary>
        /// Compares two NotifyPriorityCharging requests for inequality.
        /// </summary>
        /// <param name="NotifyPriorityChargingRequest1">A NotifyPriorityCharging request.</param>
        /// <param name="NotifyPriorityChargingRequest2">Another NotifyPriorityCharging request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyPriorityChargingRequest? NotifyPriorityChargingRequest1,
                                           NotifyPriorityChargingRequest? NotifyPriorityChargingRequest2)

            => !(NotifyPriorityChargingRequest1 == NotifyPriorityChargingRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyPriorityChargingRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyPriorityCharging requests for equality.
        /// </summary>
        /// <param name="Object">A NotifyPriorityCharging request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyPriorityChargingRequest notifyPriorityChargingRequest &&
                   Equals(notifyPriorityChargingRequest);

        #endregion

        #region Equals(NotifyPriorityChargingRequest)

        /// <summary>
        /// Compares two NotifyPriorityCharging requests for equality.
        /// </summary>
        /// <param name="NotifyPriorityChargingRequest">A NotifyPriorityCharging request to compare with.</param>
        public override Boolean Equals(NotifyPriorityChargingRequest? NotifyPriorityChargingRequest)

            => NotifyPriorityChargingRequest is not null &&

               TransactionId.Equals(NotifyPriorityChargingRequest.TransactionId) &&
               Activated.    Equals(NotifyPriorityChargingRequest.Activated)     &&

               base.GenericEquals(NotifyPriorityChargingRequest);

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

            => $"'{TransactionId}' was {(Activated ? "activated" : "deactivated")}";

        #endregion

    }

}
