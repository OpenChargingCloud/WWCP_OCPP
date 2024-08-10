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
    /// A pull dynamic schedule update request.
    /// </summary>
    public class PullDynamicScheduleUpdateRequest : ARequest<PullDynamicScheduleUpdateRequest>,
                                                    IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/pullDynamicScheduleUpdateRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification of the charging profile for which an update is requested.
        /// </summary>
        [Mandatory]
        public ChargingProfile_Id  ChargingProfileId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a pull dynamic schedule update request.
        /// </summary>
        /// <param name="DestinationId">The destination networking node identification.</param>
        /// <param name="ChargingProfileId">The identification of the charging profile for which an update is requested.</param>
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
        public PullDynamicScheduleUpdateRequest(NetworkingNode_Id        DestinationId,
                                                ChargingProfile_Id       ChargingProfileId,

                                                IEnumerable<KeyPair>?    SignKeys            = null,
                                                IEnumerable<SignInfo>?   SignInfos           = null,
                                                IEnumerable<Signature>?       Signatures          = null,

                                                CustomData?              CustomData          = null,

                                                Request_Id?              RequestId           = null,
                                                DateTime?                RequestTimestamp    = null,
                                                TimeSpan?                RequestTimeout      = null,
                                                EventTracking_Id?        EventTrackingId     = null,
                                                NetworkPath?             NetworkPath         = null,
                                                CancellationToken        CancellationToken   = default)

            : base(DestinationId,
                   nameof(PullDynamicScheduleUpdateRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            this.ChargingProfileId = ChargingProfileId;

            unchecked
            {
                hashCode = ChargingProfileId.GetHashCode() * 3 ^
                           base.             GetHashCode();
            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, DestinationId, NetworkPath, CustomPullDynamicScheduleUpdateRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a pull dynamic schedule update request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomPullDynamicScheduleUpdateRequestParser">A delegate to parse custom pull dynamic schedule update requests.</param>
        public static PullDynamicScheduleUpdateRequest Parse(JObject                                                         JSON,
                                                             Request_Id                                                      RequestId,
                                                             NetworkingNode_Id                                               DestinationId,
                                                             NetworkPath                                                     NetworkPath,
                                                             DateTime?                                                       RequestTimestamp                               = null,
                                                             TimeSpan?                                                       RequestTimeout                                 = null,
                                                             EventTracking_Id?                                               EventTrackingId                                = null,
                                                             CustomJObjectParserDelegate<PullDynamicScheduleUpdateRequest>?  CustomPullDynamicScheduleUpdateRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         DestinationId,
                         NetworkPath,
                         out var pullDynamicScheduleUpdateRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomPullDynamicScheduleUpdateRequestParser))
            {
                return pullDynamicScheduleUpdateRequest;
            }

            throw new ArgumentException("The given JSON representation of a pull dynamic schedule update request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, DestinationId, NetworkPath, out PullDynamicScheduleUpdateRequest, out ErrorResponse, CustomPullDynamicScheduleUpdateRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a pull dynamic schedule update request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="PullDynamicScheduleUpdateRequest">The parsed pull dynamic schedule update request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomPullDynamicScheduleUpdateRequestParser">A delegate to parse custom pull dynamic schedule update requests.</param>
        public static Boolean TryParse(JObject                                                         JSON,
                                       Request_Id                                                      RequestId,
                                       NetworkingNode_Id                                               DestinationId,
                                       NetworkPath                                                     NetworkPath,
                                       [NotNullWhen(true)]  out PullDynamicScheduleUpdateRequest?      PullDynamicScheduleUpdateRequest,
                                       [NotNullWhen(false)] out String?                                ErrorResponse,
                                       DateTime?                                                       RequestTimestamp                               = null,
                                       TimeSpan?                                                       RequestTimeout                                 = null,
                                       EventTracking_Id?                                               EventTrackingId                                = null,
                                       CustomJObjectParserDelegate<PullDynamicScheduleUpdateRequest>?  CustomPullDynamicScheduleUpdateRequestParser   = null)
        {

            try
            {

                PullDynamicScheduleUpdateRequest = null;

                #region ChargingProfileId    [mandatory]

                if (!JSON.ParseMandatory("chargingProfileId",
                                         "charging profile identification",
                                         ChargingProfile_Id.TryParse,
                                         out ChargingProfile_Id ChargingProfileId,
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                PullDynamicScheduleUpdateRequest = new PullDynamicScheduleUpdateRequest(

                                                       DestinationId,
                                                       ChargingProfileId,

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

                if (CustomPullDynamicScheduleUpdateRequestParser is not null)
                    PullDynamicScheduleUpdateRequest = CustomPullDynamicScheduleUpdateRequestParser(JSON,
                                                                                                    PullDynamicScheduleUpdateRequest);

                return true;

            }
            catch (Exception e)
            {
                PullDynamicScheduleUpdateRequest  = null;
                ErrorResponse                     = "The given JSON representation of a pull dynamic schedule update request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPullDynamicScheduleUpdateRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPullDynamicScheduleUpdateRequestSerializer">A delegate to serialize custom PullDynamicScheduleUpdate requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PullDynamicScheduleUpdateRequest>?  CustomPullDynamicScheduleUpdateRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                         CustomSignatureSerializer                          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                        CustomCustomDataSerializer                         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("chargingProfileId",   ChargingProfileId.ToString()),

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.       ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomPullDynamicScheduleUpdateRequestSerializer is not null
                       ? CustomPullDynamicScheduleUpdateRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (PullDynamicScheduleUpdateRequest1, PullDynamicScheduleUpdateRequest2)

        /// <summary>
        /// Compares two pull dynamic schedule update requests for equality.
        /// </summary>
        /// <param name="PullDynamicScheduleUpdateRequest1">A pull dynamic schedule update request.</param>
        /// <param name="PullDynamicScheduleUpdateRequest2">Another pull dynamic schedule update request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PullDynamicScheduleUpdateRequest? PullDynamicScheduleUpdateRequest1,
                                           PullDynamicScheduleUpdateRequest? PullDynamicScheduleUpdateRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PullDynamicScheduleUpdateRequest1, PullDynamicScheduleUpdateRequest2))
                return true;

            // If one is null, but not both, return false.
            if (PullDynamicScheduleUpdateRequest1 is null || PullDynamicScheduleUpdateRequest2 is null)
                return false;

            return PullDynamicScheduleUpdateRequest1.Equals(PullDynamicScheduleUpdateRequest2);

        }

        #endregion

        #region Operator != (PullDynamicScheduleUpdateRequest1, PullDynamicScheduleUpdateRequest2)

        /// <summary>
        /// Compares two pull dynamic schedule update requests for inequality.
        /// </summary>
        /// <param name="PullDynamicScheduleUpdateRequest1">A pull dynamic schedule update request.</param>
        /// <param name="PullDynamicScheduleUpdateRequest2">Another pull dynamic schedule update request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (PullDynamicScheduleUpdateRequest? PullDynamicScheduleUpdateRequest1,
                                           PullDynamicScheduleUpdateRequest? PullDynamicScheduleUpdateRequest2)

            => !(PullDynamicScheduleUpdateRequest1 == PullDynamicScheduleUpdateRequest2);

        #endregion

        #endregion

        #region IEquatable<PullDynamicScheduleUpdateRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two pull dynamic schedule update requests for equality.
        /// </summary>
        /// <param name="Object">A pull dynamic schedule update request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PullDynamicScheduleUpdateRequest pullDynamicScheduleUpdateRequest &&
                   Equals(pullDynamicScheduleUpdateRequest);

        #endregion

        #region Equals(PullDynamicScheduleUpdateRequest)

        /// <summary>
        /// Compares two pull dynamic schedule update requests for equality.
        /// </summary>
        /// <param name="PullDynamicScheduleUpdateRequest">A pull dynamic schedule update request to compare with.</param>
        public override Boolean Equals(PullDynamicScheduleUpdateRequest? PullDynamicScheduleUpdateRequest)

            => PullDynamicScheduleUpdateRequest is not null &&

               ChargingProfileId.Equals(PullDynamicScheduleUpdateRequest.ChargingProfileId) &&

               base.GenericEquals(PullDynamicScheduleUpdateRequest);

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

            => ChargingProfileId.ToString();

        #endregion

    }

}
