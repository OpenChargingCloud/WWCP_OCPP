/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A notify priority charging request.
    /// </summary>
    public class NotifyPriorityChargingRequest : ARequest<NotifyPriorityChargingRequest>
    {

        #region Properties

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
        /// Create a notify priority charging request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public NotifyPriorityChargingRequest(ChargeBox_Id             ChargeBoxId,
                                             Transaction_Id           TransactionId,
                                             Boolean                  Activated,

                                             IEnumerable<Signature>?  Signatures          = null,
                                             CustomData?              CustomData          = null,

                                             Request_Id?              RequestId           = null,
                                             DateTime?                RequestTimestamp    = null,
                                             TimeSpan?                RequestTimeout      = null,
                                             EventTracking_Id?        EventTrackingId     = null,
                                             CancellationToken        CancellationToken   = default)

            : base(ChargeBoxId,
                   "NotifyPriorityCharging",
                   Signatures,
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
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

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomNotifyPriorityChargingRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify priority charging request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomNotifyPriorityChargingRequestParser">A delegate to parse custom notify priority charging requests.</param>
        public static NotifyPriorityChargingRequest Parse(JObject                                                      JSON,
                                                          Request_Id                                                   RequestId,
                                                          ChargeBox_Id                                                 ChargeBoxId,
                                                          CustomJObjectParserDelegate<NotifyPriorityChargingRequest>?  CustomNotifyPriorityChargingRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var notifyPriorityChargingRequest,
                         out var errorResponse,
                         CustomNotifyPriorityChargingRequestParser))
            {
                return notifyPriorityChargingRequest!;
            }

            throw new ArgumentException("The given JSON representation of a notify priority charging request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out NotifyPriorityChargingRequest, out ErrorResponse, CustomNotifyPriorityChargingRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify priority charging request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="NotifyPriorityChargingRequest">The parsed notify priority charging request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyPriorityChargingRequestParser">A delegate to parse custom notify priority charging requests.</param>
        public static Boolean TryParse(JObject                                                      JSON,
                                       Request_Id                                                   RequestId,
                                       ChargeBox_Id                                                 ChargeBoxId,
                                       out NotifyPriorityChargingRequest?                           NotifyPriorityChargingRequest,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyPriorityChargingRequest>?  CustomNotifyPriorityChargingRequestParser)
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
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId      [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                NotifyPriorityChargingRequest = new NotifyPriorityChargingRequest(
                                                    ChargeBoxId,
                                                    TransactionId,
                                                    Activated,
                                                    Signatures,
                                                    CustomData,
                                                    RequestId
                                                );

                if (CustomNotifyPriorityChargingRequestParser is not null)
                    NotifyPriorityChargingRequest = CustomNotifyPriorityChargingRequestParser(JSON,
                                                                                              NotifyPriorityChargingRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyPriorityChargingRequest  = null;
                ErrorResponse                  = "The given JSON representation of a notify priority charging request is invalid: " + e.Message;
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

                                 new JProperty("transactionId",    TransactionId.ToString()),
                                 new JProperty("activated",        Activated),

                           Signatures.Any()
                               ? new JProperty("signatures",       new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.   ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyPriorityChargingRequestSerializer is not null
                       ? CustomNotifyPriorityChargingRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyPriorityChargingRequest1, NotifyPriorityChargingRequest2)

        /// <summary>
        /// Compares two notify priority charging requests for equality.
        /// </summary>
        /// <param name="NotifyPriorityChargingRequest1">A notify priority charging request.</param>
        /// <param name="NotifyPriorityChargingRequest2">Another notify priority charging request.</param>
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
        /// Compares two notify priority charging requests for inequality.
        /// </summary>
        /// <param name="NotifyPriorityChargingRequest1">A notify priority charging request.</param>
        /// <param name="NotifyPriorityChargingRequest2">Another notify priority charging request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyPriorityChargingRequest? NotifyPriorityChargingRequest1,
                                           NotifyPriorityChargingRequest? NotifyPriorityChargingRequest2)

            => !(NotifyPriorityChargingRequest1 == NotifyPriorityChargingRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyPriorityChargingRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify priority charging requests for equality.
        /// </summary>
        /// <param name="Object">A notify priority charging request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyPriorityChargingRequest notifyPriorityChargingRequest &&
                   Equals(notifyPriorityChargingRequest);

        #endregion

        #region Equals(NotifyPriorityChargingRequest)

        /// <summary>
        /// Compares two notify priority charging requests for equality.
        /// </summary>
        /// <param name="NotifyPriorityChargingRequest">A notify priority charging request to compare with.</param>
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
