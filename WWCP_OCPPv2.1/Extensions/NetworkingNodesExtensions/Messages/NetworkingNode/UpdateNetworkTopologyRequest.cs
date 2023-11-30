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

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// An update network topology request.
    /// </summary>
    public class UpdateNetworkTopologyRequest : ARequest<UpdateNetworkTopologyRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/updateNetworkTopologyRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The signature policy.
        /// </summary>
        [Mandatory]
        public SignaturePolicy  SignaturePolicy    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new add signature policy request.
        /// </summary>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="SignaturePolicy">A signature policy.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public UpdateNetworkTopologyRequest(ChargingStation_Id       ChargingStationId,
                                            SignaturePolicy          SignaturePolicy,

                                            IEnumerable<KeyPair>?    SignKeys            = null,
                                            IEnumerable<SignInfo>?   SignInfos           = null,
                                            IEnumerable<Signature>?  Signatures          = null,

                                            CustomData?              CustomData          = null,

                                            Request_Id?              RequestId           = null,
                                            DateTime?                RequestTimestamp    = null,
                                            TimeSpan?                RequestTimeout      = null,
                                            EventTracking_Id?        EventTrackingId     = null,
                                            CancellationToken        CancellationToken   = default)

            : base(ChargingStationId,
                   "UpdateNetworkTopology",

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.SignaturePolicy = SignaturePolicy;

            unchecked
            {

                hashCode = this.SignaturePolicy.GetHashCode() * 3 ^
                           base.                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargingStationId, CustomUpdateNetworkTopologyRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an UpdateNetworkTopology request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="CustomUpdateNetworkTopologyRequestParser">A delegate to parse custom UpdateNetworkTopology requests.</param>
        public static UpdateNetworkTopologyRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         ChargingStation_Id                                          ChargingStationId,
                                                         CustomJObjectParserDelegate<UpdateNetworkTopologyRequest>?  CustomUpdateNetworkTopologyRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         ChargingStationId,
                         out var updateNetworkTopologyRequest,
                         out var errorResponse,
                         CustomUpdateNetworkTopologyRequestParser) &&
                updateNetworkTopologyRequest is not null)
            {
                return updateNetworkTopologyRequest;
            }

            throw new ArgumentException("The given JSON representation of an UpdateNetworkTopology request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargingStationId, out UpdateNetworkTopologyRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an UpdateNetworkTopology request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="UpdateNetworkTopologyRequest">The parsed UpdateNetworkTopology request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       Request_Id                         RequestId,
                                       ChargingStation_Id                 ChargingStationId,
                                       out UpdateNetworkTopologyRequest?  UpdateNetworkTopologyRequest,
                                       out String?                        ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargingStationId,
                        out UpdateNetworkTopologyRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an UpdateNetworkTopology request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="UpdateNetworkTopologyRequest">The parsed UpdateNetworkTopology request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUpdateNetworkTopologyRequestParser">A delegate to parse custom UpdateNetworkTopology requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       ChargingStation_Id                                          ChargingStationId,
                                       out UpdateNetworkTopologyRequest?                           UpdateNetworkTopologyRequest,
                                       out String?                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<UpdateNetworkTopologyRequest>?  CustomUpdateNetworkTopologyRequestParser)
        {

            try
            {

                UpdateNetworkTopologyRequest = null;

                #region SignaturePolicy      [mandatory]

                if (!JSON.ParseMandatoryJSON("signaturePolicy",
                                             "signature policy",
                                             OCPPv2_1.SignaturePolicy.TryParse,
                                             out SignaturePolicy? SignaturePolicy,
                                             out ErrorResponse) ||
                     SignaturePolicy is null)
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingStationId    [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargingStationId",
                                       "charging station identification",
                                       ChargingStation_Id.TryParse,
                                       out ChargingStation_Id? chargingStationId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargingStationId_PayLoad.HasValue)
                        ChargingStationId = chargingStationId_PayLoad.Value;

                }

                #endregion


                UpdateNetworkTopologyRequest = new UpdateNetworkTopologyRequest(
                                                ChargingStationId,
                                                SignaturePolicy,
                                                null,
                                                null,
                                                Signatures,
                                                CustomData,
                                                RequestId
                                            );

                if (CustomUpdateNetworkTopologyRequestParser is not null)
                    UpdateNetworkTopologyRequest = CustomUpdateNetworkTopologyRequestParser(JSON,
                                                                                            UpdateNetworkTopologyRequest);

                return true;

            }
            catch (Exception e)
            {
                UpdateNetworkTopologyRequest  = null;
                ErrorResponse                 = "The given JSON representation of an UpdateNetworkTopology request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUpdateNetworkTopologyRequestSerializer = null, CustomSignaturePolicySerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateNetworkTopologyRequestSerializer">A delegate to serialize custom UpdateNetworkTopology requests.</param>
        /// <param name="CustomSignaturePolicySerializer">A delegate to serialize custom signature policies.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateNetworkTopologyRequest>?  CustomUpdateNetworkTopologyRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<SignaturePolicy>?            CustomSignaturePolicySerializer             = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("signaturePolicy",   SignaturePolicy.ToJSON(CustomSignaturePolicySerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUpdateNetworkTopologyRequestSerializer is not null
                       ? CustomUpdateNetworkTopologyRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UpdateNetworkTopologyRequest1, UpdateNetworkTopologyRequest2)

        /// <summary>
        /// Compares two UpdateNetworkTopology requests for equality.
        /// </summary>
        /// <param name="UpdateNetworkTopologyRequest1">A UpdateNetworkTopology request.</param>
        /// <param name="UpdateNetworkTopologyRequest2">Another UpdateNetworkTopology request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateNetworkTopologyRequest? UpdateNetworkTopologyRequest1,
                                           UpdateNetworkTopologyRequest? UpdateNetworkTopologyRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateNetworkTopologyRequest1, UpdateNetworkTopologyRequest2))
                return true;

            // If one is null, but not both, return false.
            if (UpdateNetworkTopologyRequest1 is null || UpdateNetworkTopologyRequest2 is null)
                return false;

            return UpdateNetworkTopologyRequest1.Equals(UpdateNetworkTopologyRequest2);

        }

        #endregion

        #region Operator != (UpdateNetworkTopologyRequest1, UpdateNetworkTopologyRequest2)

        /// <summary>
        /// Compares two UpdateNetworkTopology requests for inequality.
        /// </summary>
        /// <param name="UpdateNetworkTopologyRequest1">A UpdateNetworkTopology request.</param>
        /// <param name="UpdateNetworkTopologyRequest2">Another UpdateNetworkTopology request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateNetworkTopologyRequest? UpdateNetworkTopologyRequest1,
                                           UpdateNetworkTopologyRequest? UpdateNetworkTopologyRequest2)

            => !(UpdateNetworkTopologyRequest1 == UpdateNetworkTopologyRequest2);

        #endregion

        #endregion

        #region IEquatable<UpdateNetworkTopologyRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UpdateNetworkTopology requests for equality.
        /// </summary>
        /// <param name="Object">A UpdateNetworkTopology request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateNetworkTopologyRequest updateNetworkTopologyRequest &&
                   Equals(updateNetworkTopologyRequest);

        #endregion

        #region Equals(UpdateNetworkTopologyRequest)

        /// <summary>
        /// Compares two UpdateNetworkTopology requests for equality.
        /// </summary>
        /// <param name="UpdateNetworkTopologyRequest">A UpdateNetworkTopology request to compare with.</param>
        public override Boolean Equals(UpdateNetworkTopologyRequest? UpdateNetworkTopologyRequest)

            => UpdateNetworkTopologyRequest is not null &&

               SignaturePolicy.Equals(UpdateNetworkTopologyRequest.SignaturePolicy) &&

               base.    GenericEquals(UpdateNetworkTopologyRequest);

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

            => SignaturePolicy.ToString();

        #endregion

    }

}
