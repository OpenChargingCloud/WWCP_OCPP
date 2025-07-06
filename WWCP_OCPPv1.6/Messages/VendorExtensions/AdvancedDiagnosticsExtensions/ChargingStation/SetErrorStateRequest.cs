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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The SetErrorState request.
    /// </summary>
    public class SetErrorStateRequest : ARequest<SetErrorStateRequest>,
                                        IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/setErrorStateRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The fault type.
        /// </summary>
        [Mandatory]
        public FaultType      FaultType          { get; }

        /// <summary>
        /// The optional connector identification, when the charging station has more than one connector
        /// (0 > ConnectorId ≤ MaxConnectorId).
        /// </summary>
        [Optional]
        public Connector_Id?  ConnectorId        { get; }

        /// <summary>
        /// The optional processing delay before the request is processed by the charging station.
        /// </summary>
        [Optional]
        public TimeSpan?      ProcessingDelay    { get; }

        /// <summary>
        /// The optional duration of the error state for short transient errors.
        /// </summary>
        [Optional]
        public TimeSpan?      Duration           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetErrorState request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="FaultType">The fault type.</param>
        /// <param name="ConnectorId">An optional connector identification, when the charging station has more than one connector (0 > ConnectorId ≤ MaxConnectorId).</param>
        /// <param name="ProcessingDelay">An optional processing delay before the request is processed by the charging station.</param>
        /// <param name="Duration">An optional duration of the error state for short transient errors.</param>
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
        public SetErrorStateRequest(SourceRouting            Destination,
                                    FaultType                FaultType,
                                    Connector_Id?            ConnectorId           = null,
                                    TimeSpan?                ProcessingDelay       = null,
                                    TimeSpan?                Duration              = null,

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
                   nameof(SetErrorStateRequest)[..^7],

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

            this.FaultType        = FaultType;
            this.ConnectorId      = ConnectorId;
            this.ProcessingDelay  = ProcessingDelay;
            this.Duration         = Duration;

            unchecked
            {

                hashCode = this.FaultType.       GetHashCode()       * 11 ^
                          (this.ConnectorId?.    GetHashCode() ?? 0) *  7 ^
                          (this.ProcessingDelay?.GetHashCode() ?? 0) *  5 ^
                          (this.Duration?.       GetHashCode() ?? 0) *  3 ^
                           base.                 GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SetErrorState request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetErrorStateRequestParser">An optional delegate to parse custom SetErrorState requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static SetErrorStateRequest Parse(JObject                                             JSON,
                                                 Request_Id                                          RequestId,
                                                 SourceRouting                                       Destination,
                                                 NetworkPath                                         NetworkPath,
                                                 DateTime?                                           RequestTimestamp                   = null,
                                                 TimeSpan?                                           RequestTimeout                     = null,
                                                 EventTracking_Id?                                   EventTrackingId                    = null,
                                                 CustomJObjectParserDelegate<SetErrorStateRequest>?  CustomSetErrorStateRequestParser   = null,
                                                 CustomJObjectParserDelegate<Signature>?             CustomSignatureParser              = null,
                                                 CustomJObjectParserDelegate<CustomData>?            CustomCustomDataParser             = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setErrorStateRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetErrorStateRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setErrorStateRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetErrorState request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SetErrorStateRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SetErrorState request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetErrorStateRequest">The parsed SetErrorState request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetErrorStateRequestParser">An optional delegate to parse custom SetErrorState requests.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       Request_Id                                          RequestId,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out SetErrorStateRequest?      SetErrorStateRequest,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       DateTime?                                           RequestTimestamp                   = null,
                                       TimeSpan?                                           RequestTimeout                     = null,
                                       EventTracking_Id?                                   EventTrackingId                    = null,
                                       CustomJObjectParserDelegate<SetErrorStateRequest>?  CustomSetErrorStateRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?             CustomSignatureParser              = null,
                                       CustomJObjectParserDelegate<CustomData>?            CustomCustomDataParser             = null)
        {

            try
            {

                SetErrorStateRequest = null;

                #region FaultType          [mandatory]

                if (!JSON.ParseMandatory("faultType",
                                         "fault type",
                                         FaultType.TryParse,
                                         out FaultType faultType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId        [optional]

                if (JSON.ParseOptional("connectorId",
                                       "connector identification",
                                       Connector_Id.TryParse,
                                       out Connector_Id? connectorId,
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

                #region Duration           [optional]

                if (JSON.ParseOptionalMS("duration",
                                         "duration",
                                         out TimeSpan? duration,
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


                SetErrorStateRequest = new SetErrorStateRequest(

                                           Destination,
                                           faultType,
                                           connectorId,
                                           processingDelay,
                                           duration,

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

                if (CustomSetErrorStateRequestParser is not null)
                    SetErrorStateRequest = CustomSetErrorStateRequestParser(JSON,
                                                                            SetErrorStateRequest);

                return true;

            }
            catch (Exception e)
            {
                SetErrorStateRequest  = null;
                ErrorResponse         = "The given JSON representation of a SetErrorState request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetErrorStateRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetErrorStateRequestSerializer">A delegate to serialize custom SetErrorState requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetErrorStateRequest>?  CustomSetErrorStateRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("faultType",         FaultType.ToString()),

                           ConnectorId.    HasValue
                               ? new JProperty("connectorId",       ConnectorId.    Value.Value)
                               : null,

                           ProcessingDelay.HasValue
                               ? new JProperty("processingDelay",   ProcessingDelay.Value.Milliseconds)
                               : null,

                           Duration.       HasValue
                               ? new JProperty("duration",          Duration.       Value.Milliseconds)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetErrorStateRequestSerializer is not null
                       ? CustomSetErrorStateRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetErrorStateRequest1, SetErrorStateRequest2)

        /// <summary>
        /// Compares two SetErrorState requests for equality.
        /// </summary>
        /// <param name="SetErrorStateRequest1">A SetErrorState request.</param>
        /// <param name="SetErrorStateRequest2">Another SetErrorState request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetErrorStateRequest? SetErrorStateRequest1,
                                           SetErrorStateRequest? SetErrorStateRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetErrorStateRequest1, SetErrorStateRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetErrorStateRequest1 is null || SetErrorStateRequest2 is null)
                return false;

            return SetErrorStateRequest1.Equals(SetErrorStateRequest2);

        }

        #endregion

        #region Operator != (SetErrorStateRequest1, SetErrorStateRequest2)

        /// <summary>
        /// Compares two SetErrorState requests for inequality.
        /// </summary>
        /// <param name="SetErrorStateRequest1">A SetErrorState request.</param>
        /// <param name="SetErrorStateRequest2">Another SetErrorState request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetErrorStateRequest? SetErrorStateRequest1,
                                           SetErrorStateRequest? SetErrorStateRequest2)

            => !(SetErrorStateRequest1 == SetErrorStateRequest2);

        #endregion

        #endregion

        #region IEquatable<SetErrorStateRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetErrorState requests for equality.
        /// </summary>
        /// <param name="Object">A SetErrorState request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetErrorStateRequest setErrorStateRequest &&
                   Equals(setErrorStateRequest);

        #endregion

        #region Equals(SetErrorStateRequest)

        /// <summary>
        /// Compares two SetErrorState requests for equality.
        /// </summary>
        /// <param name="SetErrorStateRequest">A SetErrorState request to compare with.</param>
        public override Boolean Equals(SetErrorStateRequest? SetErrorStateRequest)

            => SetErrorStateRequest is not null &&

               FaultType.Equals(SetErrorStateRequest.FaultType) &&

            ((!ConnectorId.    HasValue && !SetErrorStateRequest.ConnectorId.    HasValue) ||
              (ConnectorId.    HasValue &&  SetErrorStateRequest.ConnectorId.    HasValue && ConnectorId.    Value.Equals(SetErrorStateRequest.ConnectorId.    Value))) &&

            ((!ProcessingDelay.HasValue && !SetErrorStateRequest.ProcessingDelay.HasValue) ||
              (ProcessingDelay.HasValue &&  SetErrorStateRequest.ProcessingDelay.HasValue && ProcessingDelay.Value.Equals(SetErrorStateRequest.ProcessingDelay.Value))) &&

            ((!Duration.       HasValue && !SetErrorStateRequest.Duration.       HasValue) ||
              (Duration.       HasValue &&  SetErrorStateRequest.Duration.       HasValue && Duration.       Value.Equals(SetErrorStateRequest.Duration.       Value))) &&

               base.GenericEquals(SetErrorStateRequest);

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

                   $"'{FaultType}'",

                   ConnectorId.    HasValue
                       ? $" at '{ConnectorId}'"
                       : "",

                   ProcessingDelay.HasValue
                       ? $" delay: {ProcessingDelay.Value.TotalMilliseconds} ms'"
                       : "",

                   Duration.       HasValue
                       ? $", duration: {Duration.Value.TotalMilliseconds} ms"
                       : ""

               );

        #endregion

    }

}
