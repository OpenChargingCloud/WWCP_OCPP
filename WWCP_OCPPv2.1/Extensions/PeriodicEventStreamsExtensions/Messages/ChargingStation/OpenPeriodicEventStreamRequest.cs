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
    /// An open periodic event stream request.
    /// </summary>
    public class OpenPeriodicEventStreamRequest : ARequest<OpenPeriodicEventStreamRequest>,
                                                  IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/openPeriodicEventStreamRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The constant stream data.
        /// </summary>
        [Mandatory]
        public ConstantStreamData  ConstantStreamData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new open periodic event stream request.
        /// </summary>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="ConstantStreamData">A constant stream data object.</param>
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
        public OpenPeriodicEventStreamRequest(NetworkingNode_Id        NetworkingNodeId,
                                              ConstantStreamData       ConstantStreamData,

                                              IEnumerable<KeyPair>?    SignKeys            = null,
                                              IEnumerable<SignInfo>?   SignInfos           = null,
                                              IEnumerable<Signature>?  Signatures          = null,

                                              CustomData?              CustomData          = null,

                                              Request_Id?              RequestId           = null,
                                              DateTime?                RequestTimestamp    = null,
                                              TimeSpan?                RequestTimeout      = null,
                                              EventTracking_Id?        EventTrackingId     = null,
                                              NetworkPath?             NetworkPath         = null,
                                              CancellationToken        CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(OpenPeriodicEventStreamRequest)[..^7],

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

            this.ConstantStreamData  = ConstantStreamData;

            unchecked
            {
                hashCode = this.ConstantStreamData.GetHashCode() * 3 ^
                           base.                   GetHashCode();
            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomOpenPeriodicEventStreamRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an OpenPeriodicEventStream request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomOpenPeriodicEventStreamRequestParser">A delegate to parse custom OpenPeriodicEventStream requests.</param>
        public static OpenPeriodicEventStreamRequest Parse(JObject                                                       JSON,
                                                           Request_Id                                                    RequestId,
                                                           NetworkingNode_Id                                             NetworkingNodeId,
                                                           NetworkPath                                                   NetworkPath,
                                                           CustomJObjectParserDelegate<OpenPeriodicEventStreamRequest>?  CustomOpenPeriodicEventStreamRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var openPeriodicEventStreamRequest,
                         out var errorResponse,
                         CustomOpenPeriodicEventStreamRequestParser) &&
                openPeriodicEventStreamRequest is not null)
            {
                return openPeriodicEventStreamRequest;
            }

            throw new ArgumentException("The given JSON representation of an OpenPeriodicEventStream request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out OpenPeriodicEventStreamRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an OpenPeriodicEventStream request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="OpenPeriodicEventStreamRequest">The parsed OpenPeriodicEventStream request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       Request_Id                           RequestId,
                                       NetworkingNode_Id                    NetworkingNodeId,
                                       NetworkPath                          NetworkPath,
                                       out OpenPeriodicEventStreamRequest?  OpenPeriodicEventStreamRequest,
                                       out String?                          ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out OpenPeriodicEventStreamRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an OpenPeriodicEventStream request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="OpenPeriodicEventStreamRequest">The parsed OpenPeriodicEventStream request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomOpenPeriodicEventStreamRequestParser">A delegate to parse custom OpenPeriodicEventStream requests.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       Request_Id                                                    RequestId,
                                       NetworkingNode_Id                                             NetworkingNodeId,
                                       NetworkPath                                                   NetworkPath,
                                       out OpenPeriodicEventStreamRequest?                           OpenPeriodicEventStreamRequest,
                                       out String?                                                   ErrorResponse,
                                       CustomJObjectParserDelegate<OpenPeriodicEventStreamRequest>?  CustomOpenPeriodicEventStreamRequestParser)
        {

            try
            {

                OpenPeriodicEventStreamRequest = null;

                #region ConstantStreamData    [mandatory]

                if (!JSON.ParseMandatoryJSON("constantStreamData",
                                             "constant stream data",
                                             OCPPv2_1.ConstantStreamData.TryParse,
                                             out ConstantStreamData? ConstantStreamData,
                                             out ErrorResponse) ||
                     ConstantStreamData is null)
                {
                    return false;
                }

                #endregion

                #region Signatures            [optional, OCPP_CSE]

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

                #region CustomData            [optional]

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


                OpenPeriodicEventStreamRequest = new OpenPeriodicEventStreamRequest(

                                                     NetworkingNodeId,
                                                     ConstantStreamData,

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

                if (CustomOpenPeriodicEventStreamRequestParser is not null)
                    OpenPeriodicEventStreamRequest = CustomOpenPeriodicEventStreamRequestParser(JSON,
                                                                                                OpenPeriodicEventStreamRequest);

                return true;

            }
            catch (Exception e)
            {
                OpenPeriodicEventStreamRequest  = null;
                ErrorResponse                   = "The given JSON representation of an OpenPeriodicEventStream request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomOpenPeriodicEventStreamRequestSerializer = null, CustomConstantStreamDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomOpenPeriodicEventStreamRequestSerializer">A delegate to serialize custom OpenPeriodicEventStream requests.</param>
        /// <param name="CustomConstantStreamDataSerializer">A delegate to serialize custom constant stream datas.</param>
        /// <param name="CustomPeriodicEventStreamParametersSerializer">A delegate to serialize custom periodic event stream parameterss.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<OpenPeriodicEventStreamRequest>?  CustomOpenPeriodicEventStreamRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ConstantStreamData>?              CustomConstantStreamDataSerializer               = null,
                              CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?   CustomPeriodicEventStreamParametersSerializer    = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("constantStreamData",   ConstantStreamData.ToJSON(CustomConstantStreamDataSerializer,
                                                                                                 CustomPeriodicEventStreamParametersSerializer,
                                                                                                 CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",           new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                  CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",           CustomData.        ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomOpenPeriodicEventStreamRequestSerializer is not null
                       ? CustomOpenPeriodicEventStreamRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (OpenPeriodicEventStreamRequest1, OpenPeriodicEventStreamRequest2)

        /// <summary>
        /// Compares two OpenPeriodicEventStream requests for equality.
        /// </summary>
        /// <param name="OpenPeriodicEventStreamRequest1">An OpenPeriodicEventStream request.</param>
        /// <param name="OpenPeriodicEventStreamRequest2">Another OpenPeriodicEventStream request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (OpenPeriodicEventStreamRequest? OpenPeriodicEventStreamRequest1,
                                           OpenPeriodicEventStreamRequest? OpenPeriodicEventStreamRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(OpenPeriodicEventStreamRequest1, OpenPeriodicEventStreamRequest2))
                return true;

            // If one is null, but not both, return false.
            if (OpenPeriodicEventStreamRequest1 is null || OpenPeriodicEventStreamRequest2 is null)
                return false;

            return OpenPeriodicEventStreamRequest1.Equals(OpenPeriodicEventStreamRequest2);

        }

        #endregion

        #region Operator != (OpenPeriodicEventStreamRequest1, OpenPeriodicEventStreamRequest2)

        /// <summary>
        /// Compares two OpenPeriodicEventStream requests for inequality.
        /// </summary>
        /// <param name="OpenPeriodicEventStreamRequest1">An OpenPeriodicEventStream request.</param>
        /// <param name="OpenPeriodicEventStreamRequest2">Another OpenPeriodicEventStream request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (OpenPeriodicEventStreamRequest? OpenPeriodicEventStreamRequest1,
                                           OpenPeriodicEventStreamRequest? OpenPeriodicEventStreamRequest2)

            => !(OpenPeriodicEventStreamRequest1 == OpenPeriodicEventStreamRequest2);

        #endregion

        #endregion

        #region IEquatable<OpenPeriodicEventStreamRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two OpenPeriodicEventStream requests for equality.
        /// </summary>
        /// <param name="Object">An OpenPeriodicEventStream request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is OpenPeriodicEventStreamRequest openPeriodicEventStreamRequest &&
                   Equals(openPeriodicEventStreamRequest);

        #endregion

        #region Equals(OpenPeriodicEventStreamRequest)

        /// <summary>
        /// Compares two OpenPeriodicEventStream requests for equality.
        /// </summary>
        /// <param name="OpenPeriodicEventStreamRequest">An OpenPeriodicEventStream request to compare with.</param>
        public override Boolean Equals(OpenPeriodicEventStreamRequest? OpenPeriodicEventStreamRequest)

            => OpenPeriodicEventStreamRequest is not null &&

               ConstantStreamData.Equals(OpenPeriodicEventStreamRequest.ConstantStreamData) &&

               base.       GenericEquals(OpenPeriodicEventStreamRequest);

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

            => ConstantStreamData.ToString();

        #endregion


    }

}
