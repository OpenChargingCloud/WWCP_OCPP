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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A get periodic event stream request.
    /// </summary>
    public class GetPeriodicEventStreamsRequest : ARequest<GetPeriodicEventStreamsRequest>,
                                                  IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getPeriodicEventStreamsRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get periodic event stream request.
        /// </summary>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
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
        public GetPeriodicEventStreamsRequest(SourceRouting            Destination,

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
                   nameof(GetPeriodicEventStreamsRequest)[..^7],

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

            unchecked
            {
                hashCode = base.GetHashCode();
            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomGetPeriodicEventStreamRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an GetPeriodicEventStreams request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomGetPeriodicEventStreamRequestParser">A delegate to parse custom GetPeriodicEventStreams requests.</param>
        public static GetPeriodicEventStreamsRequest Parse(JObject                                                       JSON,
                                                           Request_Id                                                    RequestId,
                                                           SourceRouting                                             Destination,
                                                           NetworkPath                                                   NetworkPath,
                                                           CustomJObjectParserDelegate<GetPeriodicEventStreamsRequest>?  CustomGetPeriodicEventStreamRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getPeriodicEventStreamsRequest,
                         out var errorResponse,
                         CustomGetPeriodicEventStreamRequestParser) &&
                getPeriodicEventStreamsRequest is not null)
            {
                return getPeriodicEventStreamsRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetPeriodicEventStreams request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out GetPeriodicEventStreamRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a GetPeriodicEventStreams request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetPeriodicEventStreamRequest">The parsed GetPeriodicEventStreams request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       Request_Id                           RequestId,
                                       SourceRouting                    Destination,
                                       NetworkPath                          NetworkPath,
                                       out GetPeriodicEventStreamsRequest?  GetPeriodicEventStreamRequest,
                                       out String?                          ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        Destination,
                        NetworkPath,
                        out GetPeriodicEventStreamRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a GetPeriodicEventStreams request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetPeriodicEventStreamRequest">The parsed GetPeriodicEventStreams request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetPeriodicEventStreamRequestParser">A delegate to parse custom GetPeriodicEventStreams requests.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       Request_Id                                                    RequestId,
                                       SourceRouting                                             Destination,
                                       NetworkPath                                                   NetworkPath,
                                       out GetPeriodicEventStreamsRequest?                           GetPeriodicEventStreamRequest,
                                       out String?                                                   ErrorResponse,
                                       CustomJObjectParserDelegate<GetPeriodicEventStreamsRequest>?  CustomGetPeriodicEventStreamRequestParser)
        {

            try
            {

                GetPeriodicEventStreamRequest = null;

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


                GetPeriodicEventStreamRequest = new GetPeriodicEventStreamsRequest(

                                                    Destination,

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

                if (CustomGetPeriodicEventStreamRequestParser is not null)
                    GetPeriodicEventStreamRequest = CustomGetPeriodicEventStreamRequestParser(JSON,
                                                                                              GetPeriodicEventStreamRequest);

                return true;

            }
            catch (Exception e)
            {
                GetPeriodicEventStreamRequest  = null;
                ErrorResponse                  = "The given JSON representation of a GetPeriodicEventStreams request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetPeriodicEventStreamRequestSerializer = null, CustomSignaturePolicySerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetPeriodicEventStreamRequestSerializer">A delegate to serialize custom GetPeriodicEventStreams requests.</param>
        /// <param name="CustomSignaturePolicySerializer">A delegate to serialize custom signature policies.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetPeriodicEventStreamsRequest>?  CustomGetPeriodicEventStreamRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<SignaturePolicy>?                 CustomSignaturePolicySerializer                 = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetPeriodicEventStreamRequestSerializer is not null
                       ? CustomGetPeriodicEventStreamRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetPeriodicEventStreamRequest1, GetPeriodicEventStreamRequest2)

        /// <summary>
        /// Compares two GetPeriodicEventStreams requests for equality.
        /// </summary>
        /// <param name="GetPeriodicEventStreamRequest1">A GetPeriodicEventStreams request.</param>
        /// <param name="GetPeriodicEventStreamRequest2">Another GetPeriodicEventStreams request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetPeriodicEventStreamsRequest? GetPeriodicEventStreamRequest1,
                                           GetPeriodicEventStreamsRequest? GetPeriodicEventStreamRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetPeriodicEventStreamRequest1, GetPeriodicEventStreamRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetPeriodicEventStreamRequest1 is null || GetPeriodicEventStreamRequest2 is null)
                return false;

            return GetPeriodicEventStreamRequest1.Equals(GetPeriodicEventStreamRequest2);

        }

        #endregion

        #region Operator != (GetPeriodicEventStreamRequest1, GetPeriodicEventStreamRequest2)

        /// <summary>
        /// Compares two GetPeriodicEventStreams requests for inequality.
        /// </summary>
        /// <param name="GetPeriodicEventStreamRequest1">A GetPeriodicEventStreams request.</param>
        /// <param name="GetPeriodicEventStreamRequest2">Another GetPeriodicEventStreams request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetPeriodicEventStreamsRequest? GetPeriodicEventStreamRequest1,
                                           GetPeriodicEventStreamsRequest? GetPeriodicEventStreamRequest2)

            => !(GetPeriodicEventStreamRequest1 == GetPeriodicEventStreamRequest2);

        #endregion

        #endregion

        #region IEquatable<GetPeriodicEventStreamRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetPeriodicEventStreams requests for equality.
        /// </summary>
        /// <param name="Object">A GetPeriodicEventStreams request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetPeriodicEventStreamsRequest getPeriodicEventStreamsRequest &&
                   Equals(getPeriodicEventStreamsRequest);

        #endregion

        #region Equals(GetPeriodicEventStreamRequest)

        /// <summary>
        /// Compares two GetPeriodicEventStreams requests for equality.
        /// </summary>
        /// <param name="GetPeriodicEventStreamRequest">A GetPeriodicEventStreams request to compare with.</param>
        public override Boolean Equals(GetPeriodicEventStreamsRequest? GetPeriodicEventStreamRequest)

            => GetPeriodicEventStreamRequest is not null &&

               base.GenericEquals(GetPeriodicEventStreamRequest);

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

            => "";

        #endregion


    }

}
