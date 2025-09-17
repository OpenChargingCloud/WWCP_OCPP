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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The SwipeRFIDCard request.
    /// </summary>
    public class SwipeRFIDCardRequest : ARequest<SwipeRFIDCardRequest>,
                                        IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/swipeRFIDCardRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext           Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification token, e.g. of the RFID card to be swiped.
        /// </summary>
        [Mandatory]
        public IdToken                 IdToken            { get; }

        /// <summary>
        /// The optional RFID reader identification, when the charging station has more than one connector
        /// and therefore more than one RFID reader or an additional user interface process to select a
        /// specific connector before or after swiping the RFID card (0 > ReaderId ≤ MaxConnectorId).
        /// </summary>
        [Optional]
        public EVSE_Id?                ReaderId           { get; }

        /// <summary>
        /// The optional simulation mode: Software | Hardware | ...
        /// </summary>
        [Optional]
        public IdTokenSimulationMode?  SimulationMode     { get; }

        /// <summary>
        /// The optional processing delay before the request is processed by the charging station.
        /// </summary>
        [Optional]
        public TimeSpan?               ProcessingDelay    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SwipeRFIDCard request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="IdToken">The identification token, e.g. of the RFID card to be swiped.</param>
        /// <param name="ReaderId">An optional RFID reader identification, when the charging station has more than one connector and therefore more than one RFID reader or an additional user interface process to select a specific connector before or after swiping the RFID card (0 > ReaderId ≤ MaxConnectorId).
        /// <param name="SimulationMode">An optional simulation mode: Software | Hardware | ...</param>
        /// <param name="ProcessingDelay">An optional processing delay before the request is processed by the charging station.</param>
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
        public SwipeRFIDCardRequest(SourceRouting            Destination,
                                    IdToken                  IdToken,
                                    EVSE_Id?                 ReaderId              = null,
                                    IdTokenSimulationMode?   SimulationMode        = null,
                                    TimeSpan?                ProcessingDelay       = null,

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
                   nameof(SwipeRFIDCardRequest)[..^7],

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

            this.IdToken          = IdToken;
            this.ReaderId         = ReaderId;
            this.SimulationMode   = SimulationMode;
            this.ProcessingDelay  = ProcessingDelay;

            unchecked
            {

                hashCode = this.IdToken.         GetHashCode()       * 11 ^
                          (this.ReaderId?.       GetHashCode() ?? 0) *  7 ^
                          (this.SimulationMode?. GetHashCode() ?? 0) *  5 ^
                          (this.ProcessingDelay?.GetHashCode() ?? 0) *  3 ^
                           base.                 GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SwipeRFIDCard request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSwipeRFIDCardRequestParser">An optional delegate to parse custom SwipeRFIDCard requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static SwipeRFIDCardRequest Parse(JObject                                                JSON,
                                                    Request_Id                                          RequestId,
                                                    SourceRouting                                       Destination,
                                                    NetworkPath                                         NetworkPath,
                                                    DateTimeOffset?                                     RequestTimestamp                   = null,
                                                    TimeSpan?                                           RequestTimeout                     = null,
                                                    EventTracking_Id?                                   EventTrackingId                    = null,
                                                    CustomJObjectParserDelegate<SwipeRFIDCardRequest>?  CustomSwipeRFIDCardRequestParser   = null,
                                                    CustomJObjectParserDelegate<Signature>?             CustomSignatureParser              = null,
                                                    CustomJObjectParserDelegate<CustomData>?            CustomCustomDataParser             = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var swipeRFIDCardRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSwipeRFIDCardRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return swipeRFIDCardRequest;
            }

            throw new ArgumentException("The given JSON representation of a SwipeRFIDCard request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SwipeRFIDCardRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SwipeRFIDCard request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SwipeRFIDCardRequest">The parsed SwipeRFIDCard request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSwipeRFIDCardRequestParser">An optional delegate to parse custom SwipeRFIDCard requests.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       Request_Id                                          RequestId,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out SwipeRFIDCardRequest?      SwipeRFIDCardRequest,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       DateTimeOffset?                                     RequestTimestamp                   = null,
                                       TimeSpan?                                           RequestTimeout                     = null,
                                       EventTracking_Id?                                   EventTrackingId                    = null,
                                       CustomJObjectParserDelegate<SwipeRFIDCardRequest>?  CustomSwipeRFIDCardRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?             CustomSignatureParser              = null,
                                       CustomJObjectParserDelegate<CustomData>?            CustomCustomDataParser             = null)
        {

            try
            {

                SwipeRFIDCardRequest = null;

                #region IdToken            [mandatory]

                if (!JSON.ParseMandatoryJSON("idToken",
                                             "identification token",
                                             OCPPv2_1.IdToken.TryParse,
                                             out IdToken? idToken,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ReaderId           [optional]

                if (JSON.ParseOptional("readerId",
                                       "reader identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id? readerId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region SimulationMode     [optional]

                if (JSON.ParseOptional("simulationMode",
                                       "simulation mode",
                                       IdTokenSimulationMode.TryParse,
                                       out IdTokenSimulationMode? simulationMode,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ProcessingDelay    [optional]

                if (JSON.ParseOptionalMS("processingDelay",
                                         "processing delay",
                                         out TimeSpan? processingDelay,
                                         out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures         [optional, OCPP_CSE]

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

                #region CustomData         [optional]

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


                SwipeRFIDCardRequest = new SwipeRFIDCardRequest(

                                           Destination,
                                           idToken,
                                           readerId,
                                           simulationMode,
                                           processingDelay,

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

                if (CustomSwipeRFIDCardRequestParser is not null)
                    SwipeRFIDCardRequest = CustomSwipeRFIDCardRequestParser(JSON,
                                                                            SwipeRFIDCardRequest);

                return true;

            }
            catch (Exception e)
            {
                SwipeRFIDCardRequest  = null;
                ErrorResponse         = "The given JSON representation of a SwipeRFIDCard request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSwipeRFIDCardRequestSerializer = null, CustomIdTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSwipeRFIDCardRequestSerializer">A delegate to serialize custom SwipeRFIDCard requests.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SwipeRFIDCardRequest>?  CustomSwipeRFIDCardRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?               CustomIdTokenSerializer                = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?        CustomAdditionalInfoSerializer         = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("idToken",           IdToken.ToJSON(CustomIdTokenSerializer,
                                                                                   CustomAdditionalInfoSerializer,
                                                                                   CustomCustomDataSerializer)),

                           ReaderId.       HasValue
                               ? new JProperty("readerId",          ReaderId.Value.Value)
                               : null,

                           SimulationMode. HasValue
                               ? new JProperty("simulationMode",    SimulationMode.ToString())
                               : null,

                           ProcessingDelay.HasValue
                               ? new JProperty("processingDelay",   ProcessingDelay.Value.Milliseconds)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSwipeRFIDCardRequestSerializer is not null
                       ? CustomSwipeRFIDCardRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SwipeRFIDCardRequest1, SwipeRFIDCardRequest2)

        /// <summary>
        /// Compares two SwipeRFIDCard requests for equality.
        /// </summary>
        /// <param name="SwipeRFIDCardRequest1">A SwipeRFIDCard request.</param>
        /// <param name="SwipeRFIDCardRequest2">Another SwipeRFIDCard request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SwipeRFIDCardRequest? SwipeRFIDCardRequest1,
                                           SwipeRFIDCardRequest? SwipeRFIDCardRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SwipeRFIDCardRequest1, SwipeRFIDCardRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SwipeRFIDCardRequest1 is null || SwipeRFIDCardRequest2 is null)
                return false;

            return SwipeRFIDCardRequest1.Equals(SwipeRFIDCardRequest2);

        }

        #endregion

        #region Operator != (SwipeRFIDCardRequest1, SwipeRFIDCardRequest2)

        /// <summary>
        /// Compares two SwipeRFIDCard requests for inequality.
        /// </summary>
        /// <param name="SwipeRFIDCardRequest1">A SwipeRFIDCard request.</param>
        /// <param name="SwipeRFIDCardRequest2">Another SwipeRFIDCard request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SwipeRFIDCardRequest? SwipeRFIDCardRequest1,
                                           SwipeRFIDCardRequest? SwipeRFIDCardRequest2)

            => !(SwipeRFIDCardRequest1 == SwipeRFIDCardRequest2);

        #endregion

        #endregion

        #region IEquatable<SwipeRFIDCardRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SwipeRFIDCard requests for equality.
        /// </summary>
        /// <param name="Object">A SwipeRFIDCard request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SwipeRFIDCardRequest swipeRFIDCardRequest &&
                   Equals(swipeRFIDCardRequest);

        #endregion

        #region Equals(SwipeRFIDCardRequest)

        /// <summary>
        /// Compares two SwipeRFIDCard requests for equality.
        /// </summary>
        /// <param name="SwipeRFIDCardRequest">A SwipeRFIDCard request to compare with.</param>
        public override Boolean Equals(SwipeRFIDCardRequest? SwipeRFIDCardRequest)

            => SwipeRFIDCardRequest is not null &&

               IdToken.Equals(SwipeRFIDCardRequest.IdToken) &&

            ((!ReaderId.       HasValue && !SwipeRFIDCardRequest.ReaderId.       HasValue) ||
             ( ReaderId.       HasValue &&  SwipeRFIDCardRequest.ReaderId.       HasValue && ReaderId.       Value.Equals(SwipeRFIDCardRequest.ReaderId.       Value))) &&

            ((!SimulationMode. HasValue && !SwipeRFIDCardRequest.SimulationMode. HasValue) ||
             ( SimulationMode. HasValue &&  SwipeRFIDCardRequest.SimulationMode. HasValue && SimulationMode. Value.Equals(SwipeRFIDCardRequest.SimulationMode. Value))) &&

            ((!ProcessingDelay.HasValue && !SwipeRFIDCardRequest.ProcessingDelay.HasValue) ||
             ( ProcessingDelay.HasValue &&  SwipeRFIDCardRequest.ProcessingDelay.HasValue && ProcessingDelay.Value.Equals(SwipeRFIDCardRequest.ProcessingDelay.Value))) &&

               base.GenericEquals(SwipeRFIDCardRequest);

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

            => String.Concat(

                   $"IdToken: '{IdToken}'",

                   ReaderId.HasValue
                       ? $" on reader {ReaderId}"
                       : "",

                   SimulationMode.HasValue
                       ? $", '{SimulationMode}' simulation"
                       : "",

                   ProcessingDelay.HasValue
                       ? $", processing delay: {ProcessingDelay.Value.TotalMilliseconds} ms"
                       : ""

               );

        #endregion

    }

}
