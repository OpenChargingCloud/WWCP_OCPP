/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/GetChargingCloud/WWCP_OCPP>
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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A ThrottlePeriodicEventStream request.
    /// </summary>
    public class ThrottlePeriodicEventStreamRequest : ARequest<ThrottlePeriodicEventStreamRequest>,
                                                      IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/ThrottlePeriodicEventStreamsRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of the data stream to throttle.
        /// </summary>
        [Mandatory]
        public PeriodicEventStream_Id         StreamId      { get; }

        /// <summary>
        /// The updated rate of sending data.
        /// </summary>
        [Mandatory]
        public PeriodicEventStreamParameters  Parameters    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get periodic event stream request.
        /// </summary>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="StreamId">The unique identification of the data stream to throttle.</param>
        /// <param name="Parameters">The updated rate of sending data.</param>
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
        public ThrottlePeriodicEventStreamRequest(NetworkingNode_Id              DestinationId,
                                                  PeriodicEventStream_Id         StreamId,
                                                  PeriodicEventStreamParameters  Parameters,

                                                  IEnumerable<KeyPair>?          SignKeys            = null,
                                                  IEnumerable<SignInfo>?         SignInfos           = null,
                                                  IEnumerable<Signature>?        Signatures          = null,

                                                  CustomData?                    CustomData          = null,

                                                  Request_Id?                    RequestId           = null,
                                                  DateTime?                      RequestTimestamp    = null,
                                                  TimeSpan?                      RequestTimeout      = null,
                                                  EventTracking_Id?              EventTrackingId     = null,
                                                  NetworkPath?                   NetworkPath         = null,
                                                  CancellationToken              CancellationToken   = default)

            : base(DestinationId,
                   nameof(ThrottlePeriodicEventStreamRequest)[..^7],

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

            this.StreamId    = StreamId;
            this.Parameters  = Parameters;

            unchecked
            {
                hashCode = this.StreamId.  GetHashCode() * 5 ^
                           this.Parameters.GetHashCode() * 3 ^
                           base.           GetHashCode();
            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, DestinationId, NetworkPath, CustomThrottlePeriodicEventStreamRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an ThrottlePeriodicEventStreams request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomThrottlePeriodicEventStreamRequestParser">A delegate to parse custom ThrottlePeriodicEventStreams requests.</param>
        public static ThrottlePeriodicEventStreamRequest Parse(JObject                                                           JSON,
                                                               Request_Id                                                        RequestId,
                                                               NetworkingNode_Id                                                 DestinationId,
                                                               NetworkPath                                                       NetworkPath,
                                                               CustomJObjectParserDelegate<ThrottlePeriodicEventStreamRequest>?  CustomThrottlePeriodicEventStreamRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         DestinationId,
                         NetworkPath,
                         out var throttlePeriodicEventStreamsRequest,
                         out var errorResponse,
                         CustomThrottlePeriodicEventStreamRequestParser) &&
                throttlePeriodicEventStreamsRequest is not null)
            {
                return throttlePeriodicEventStreamsRequest;
            }

            throw new ArgumentException("The given JSON representation of a ThrottlePeriodicEventStreams request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, DestinationId, NetworkPath, out ThrottlePeriodicEventStreamRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a ThrottlePeriodicEventStreams request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ThrottlePeriodicEventStreamRequest">The parsed ThrottlePeriodicEventStreams request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       Request_Id                                                    RequestId,
                                       NetworkingNode_Id                                             DestinationId,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out ThrottlePeriodicEventStreamRequest?  ThrottlePeriodicEventStreamRequest,
                                       [NotNullWhen(false)] out String?                              ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        DestinationId,
                        NetworkPath,
                        out ThrottlePeriodicEventStreamRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a ThrottlePeriodicEventStreams request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ThrottlePeriodicEventStreamRequest">The parsed ThrottlePeriodicEventStreams request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomThrottlePeriodicEventStreamRequestParser">A delegate to parse custom ThrottlePeriodicEventStreams requests.</param>
        public static Boolean TryParse(JObject                                                           JSON,
                                       Request_Id                                                        RequestId,
                                       NetworkingNode_Id                                                 DestinationId,
                                       NetworkPath                                                       NetworkPath,
                                       [NotNullWhen(true)]  out ThrottlePeriodicEventStreamRequest?      ThrottlePeriodicEventStreamRequest,
                                       [NotNullWhen(false)] out String?                                  ErrorResponse,
                                       CustomJObjectParserDelegate<ThrottlePeriodicEventStreamRequest>?  CustomThrottlePeriodicEventStreamRequestParser)
        {

            try
            {

                ThrottlePeriodicEventStreamRequest = null;

                #region StreamId      [mandatory]

                if (JSON.ParseMandatory("id",
                                        "unique event stream id",
                                        PeriodicEventStream_Id.TryParse,
                                        out PeriodicEventStream_Id StreamId,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parameters    [mandatory]

                if (!JSON.ParseMandatoryJSON("parameters",
                                             "event stream parameters",
                                             PeriodicEventStreamParameters.TryParse,
                                             out PeriodicEventStreamParameters? Parameters,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                ThrottlePeriodicEventStreamRequest = new ThrottlePeriodicEventStreamRequest(

                                                    DestinationId,
                                                    StreamId,
                                                    Parameters,

                                                    null,
                                                    null,
                                                    Signatures,

                                                    CustomData,

                                                    RequestId,
                                                    null,
                                                    null,
                                                    null,
                                                    NetworkPath

                                                );

                if (CustomThrottlePeriodicEventStreamRequestParser is not null)
                    ThrottlePeriodicEventStreamRequest = CustomThrottlePeriodicEventStreamRequestParser(JSON,
                                                                                              ThrottlePeriodicEventStreamRequest);

                return true;

            }
            catch (Exception e)
            {
                ThrottlePeriodicEventStreamRequest  = null;
                ErrorResponse                  = "The given JSON representation of a ThrottlePeriodicEventStreams request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomThrottlePeriodicEventStreamRequestSerializer = null, CustomPeriodicEventStreamParametersSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomThrottlePeriodicEventStreamRequestSerializer">A delegate to serialize custom ThrottlePeriodicEventStreams requests.</param>
        /// <param name="CustomPeriodicEventStreamParametersSerializer">A delegate to serialize custom periodic event stream parameterss.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ThrottlePeriodicEventStreamRequest>?  CustomThrottlePeriodicEventStreamRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?       CustomPeriodicEventStreamParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                           CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                          CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",           StreamId.Value),
                                 new JProperty("params",       Parameters.ToJSON(CustomPeriodicEventStreamParametersSerializer,
                                                                                 CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomThrottlePeriodicEventStreamRequestSerializer is not null
                       ? CustomThrottlePeriodicEventStreamRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ThrottlePeriodicEventStreamRequest1, ThrottlePeriodicEventStreamRequest2)

        /// <summary>
        /// Compares two ThrottlePeriodicEventStreams requests for equality.
        /// </summary>
        /// <param name="ThrottlePeriodicEventStreamRequest1">A ThrottlePeriodicEventStreams request.</param>
        /// <param name="ThrottlePeriodicEventStreamRequest2">Another ThrottlePeriodicEventStreams request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ThrottlePeriodicEventStreamRequest? ThrottlePeriodicEventStreamRequest1,
                                           ThrottlePeriodicEventStreamRequest? ThrottlePeriodicEventStreamRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ThrottlePeriodicEventStreamRequest1, ThrottlePeriodicEventStreamRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ThrottlePeriodicEventStreamRequest1 is null || ThrottlePeriodicEventStreamRequest2 is null)
                return false;

            return ThrottlePeriodicEventStreamRequest1.Equals(ThrottlePeriodicEventStreamRequest2);

        }

        #endregion

        #region Operator != (ThrottlePeriodicEventStreamRequest1, ThrottlePeriodicEventStreamRequest2)

        /// <summary>
        /// Compares two ThrottlePeriodicEventStreams requests for inequality.
        /// </summary>
        /// <param name="ThrottlePeriodicEventStreamRequest1">A ThrottlePeriodicEventStreams request.</param>
        /// <param name="ThrottlePeriodicEventStreamRequest2">Another ThrottlePeriodicEventStreams request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ThrottlePeriodicEventStreamRequest? ThrottlePeriodicEventStreamRequest1,
                                           ThrottlePeriodicEventStreamRequest? ThrottlePeriodicEventStreamRequest2)

            => !(ThrottlePeriodicEventStreamRequest1 == ThrottlePeriodicEventStreamRequest2);

        #endregion

        #endregion

        #region IEquatable<ThrottlePeriodicEventStreamRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ThrottlePeriodicEventStreams requests for equality.
        /// </summary>
        /// <param name="Object">A ThrottlePeriodicEventStreams request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ThrottlePeriodicEventStreamRequest ThrottlePeriodicEventStreamsRequest &&
                   Equals(ThrottlePeriodicEventStreamsRequest);

        #endregion

        #region Equals(ThrottlePeriodicEventStreamRequest)

        /// <summary>
        /// Compares two ThrottlePeriodicEventStreams requests for equality.
        /// </summary>
        /// <param name="ThrottlePeriodicEventStreamRequest">A ThrottlePeriodicEventStreams request to compare with.</param>
        public override Boolean Equals(ThrottlePeriodicEventStreamRequest? ThrottlePeriodicEventStreamRequest)

            => ThrottlePeriodicEventStreamRequest is not null &&

               StreamId.  Equals(ThrottlePeriodicEventStreamRequest.StreamId)   &&
               Parameters.Equals(ThrottlePeriodicEventStreamRequest.Parameters) &&

               base.GenericEquals(ThrottlePeriodicEventStreamRequest);

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

            => $"{StreamId}: {Parameters}";

        #endregion


    }

}
