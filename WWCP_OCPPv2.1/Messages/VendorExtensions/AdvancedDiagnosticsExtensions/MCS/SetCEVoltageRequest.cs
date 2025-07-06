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
    /// The SetCEVoltage request.
    /// </summary>
    public class SetCEVoltageRequest : ARequest<SetCEVoltageRequest>,
                                       IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/setCEVoltageRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The voltage on the Charge Pilot.
        /// </summary>
        [Mandatory]
        public Volt           Voltage           { get; }

        /// <summary>
        /// The optional random variation within ±n% to simulate real-world analog behavior.
        /// </summary>
        [Optional]
        public Percentage?    VoltageError      { get; }

        /// <summary>
        /// The optional processing delay before the request is processed by the charging station.
        /// </summary>
        [Optional]
        public TimeSpan?      ProcessingDelay    { get; }

        /// <summary>
        /// The optional gradual voltage change over the given time span avoiding instantaneous jumps
        /// to simulate real-world analog behavior.
        /// </summary>
        [Optional]
        public TimeSpan?      TransitionTime    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetCEVoltage request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Voltage">The voltage on the Charge Pilot.</param>
        /// <param name="VoltageError">An optional random variation within ±n% to simulate real-world analog behavior.</param>
        /// <param name="ProcessingDelay">An optional processing delay before the request is processed by the charging station.</param>
        /// <param name="TransitionTime">An optional gradual voltage change over the given time span avoiding instantaneous jumps to simulate real-world analog behavior.</param>
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
        public SetCEVoltageRequest(SourceRouting            Destination,
                                   Volt                     Voltage,
                                   Percentage?              VoltageError          = null,
                                   TimeSpan?                ProcessingDelay       = null,
                                   TimeSpan?                TransitionTime        = null,

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
                   nameof(SetCEVoltageRequest)[..^7],

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

            this.Voltage          = Voltage;
            this.VoltageError     = VoltageError;
            this.ProcessingDelay  = ProcessingDelay;
            this.TransitionTime   = TransitionTime;

            unchecked
            {

                hashCode = this.Voltage.         GetHashCode()       * 11 ^
                          (this.VoltageError?.   GetHashCode() ?? 0) *  7 ^
                          (this.ProcessingDelay?.GetHashCode() ?? 0) *  5 ^
                          (this.TransitionTime?. GetHashCode() ?? 0) *  3 ^
                           base.                 GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SetCEVoltage request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetCEVoltageRequestParser">An optional delegate to parse custom SetCEVoltage requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static SetCEVoltageRequest Parse(JObject                                            JSON,
                                                Request_Id                                         RequestId,
                                                SourceRouting                                      Destination,
                                                NetworkPath                                        NetworkPath,
                                                DateTime?                                          RequestTimestamp                  = null,
                                                TimeSpan?                                          RequestTimeout                    = null,
                                                EventTracking_Id?                                  EventTrackingId                   = null,
                                                CustomJObjectParserDelegate<SetCEVoltageRequest>?  CustomSetCEVoltageRequestParser   = null,
                                                CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                                CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setCEVoltageRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetCEVoltageRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setCEVoltageRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetCEVoltage request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SetCEVoltageRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SetCEVoltage request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetCEVoltageRequest">The parsed SetCEVoltage request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetCEVoltageRequestParser">An optional delegate to parse custom SetCEVoltage requests.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       Request_Id                                         RequestId,
                                       SourceRouting                                      Destination,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out SetCEVoltageRequest?      SetCEVoltageRequest,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTime?                                          RequestTimestamp                  = null,
                                       TimeSpan?                                          RequestTimeout                    = null,
                                       EventTracking_Id?                                  EventTrackingId                   = null,
                                       CustomJObjectParserDelegate<SetCEVoltageRequest>?  CustomSetCEVoltageRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                       CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            try
            {

                SetCEVoltageRequest = null;

                #region Voltage            [mandatory]

                if (!JSON.ParseMandatory("voltage",
                                         "CP voltage",
                                         Volt.TryParse,
                                         out Volt voltage,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region VoltageError       [optional]

                if (JSON.ParseOptional("voltageError",
                                       "voltage error",
                                       Percentage.TryParse,
                                       out Percentage? voltageError,
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

                #region TransitionTime     [optional]

                if (JSON.ParseOptionalMS("transitionTime",
                                         "transition time",
                                         out TimeSpan? transitionTime,
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


                SetCEVoltageRequest = new SetCEVoltageRequest(

                                          Destination,
                                          voltage,
                                          voltageError,
                                          processingDelay,
                                          transitionTime,

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

                if (CustomSetCEVoltageRequestParser is not null)
                    SetCEVoltageRequest = CustomSetCEVoltageRequestParser(JSON,
                                                                          SetCEVoltageRequest);

                return true;

            }
            catch (Exception e)
            {
                SetCEVoltageRequest  = null;
                ErrorResponse        = "The given JSON representation of a SetCEVoltage request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetCEVoltageRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetCEVoltageRequestSerializer">A delegate to serialize custom SetCEVoltage requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetCEVoltageRequest>?  CustomSetCEVoltageRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("voltage",           Voltage.        Value),

                           VoltageError.  HasValue
                               ? new JProperty("voltageError",      VoltageError.   Value.Value)
                               : null,

                           ProcessingDelay.HasValue
                               ? new JProperty("processingDelay",   ProcessingDelay.Value.Milliseconds)
                               : null,

                           TransitionTime.HasValue
                               ? new JProperty("transitionTime",    TransitionTime. Value.Milliseconds)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetCEVoltageRequestSerializer is not null
                       ? CustomSetCEVoltageRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetCEVoltageRequest1, SetCEVoltageRequest2)

        /// <summary>
        /// Compares two SetCEVoltage requests for equality.
        /// </summary>
        /// <param name="SetCEVoltageRequest1">A SetCEVoltage request.</param>
        /// <param name="SetCEVoltageRequest2">Another SetCEVoltage request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetCEVoltageRequest? SetCEVoltageRequest1,
                                           SetCEVoltageRequest? SetCEVoltageRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetCEVoltageRequest1, SetCEVoltageRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetCEVoltageRequest1 is null || SetCEVoltageRequest2 is null)
                return false;

            return SetCEVoltageRequest1.Equals(SetCEVoltageRequest2);

        }

        #endregion

        #region Operator != (SetCEVoltageRequest1, SetCEVoltageRequest2)

        /// <summary>
        /// Compares two SetCEVoltage requests for inequality.
        /// </summary>
        /// <param name="SetCEVoltageRequest1">A SetCEVoltage request.</param>
        /// <param name="SetCEVoltageRequest2">Another SetCEVoltage request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetCEVoltageRequest? SetCEVoltageRequest1,
                                           SetCEVoltageRequest? SetCEVoltageRequest2)

            => !(SetCEVoltageRequest1 == SetCEVoltageRequest2);

        #endregion

        #endregion

        #region IEquatable<SetCEVoltageRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetCEVoltage requests for equality.
        /// </summary>
        /// <param name="Object">A SetCEVoltage request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetCEVoltageRequest setCEVoltageRequest &&
                   Equals(setCEVoltageRequest);

        #endregion

        #region Equals(SetCEVoltageRequest)

        /// <summary>
        /// Compares two SetCEVoltage requests for equality.
        /// </summary>
        /// <param name="SetCEVoltageRequest">A SetCEVoltage request to compare with.</param>
        public override Boolean Equals(SetCEVoltageRequest? SetCEVoltageRequest)

            => SetCEVoltageRequest is not null &&

               Voltage.Equals(SetCEVoltageRequest.Voltage) &&

            ((!VoltageError.   HasValue && !SetCEVoltageRequest.VoltageError.   HasValue) ||
              (VoltageError.   HasValue &&  SetCEVoltageRequest.VoltageError.   HasValue && VoltageError.   Value.Equals(SetCEVoltageRequest.VoltageError.   Value))) &&

            ((!ProcessingDelay.HasValue && !SetCEVoltageRequest.ProcessingDelay.HasValue) ||
              (ProcessingDelay.HasValue &&  SetCEVoltageRequest.ProcessingDelay.HasValue && ProcessingDelay.Value.Equals(SetCEVoltageRequest.ProcessingDelay.Value))) &&

            ((!TransitionTime. HasValue && !SetCEVoltageRequest.TransitionTime. HasValue) ||
              (TransitionTime. HasValue &&  SetCEVoltageRequest.TransitionTime. HasValue && TransitionTime. Value.Equals(SetCEVoltageRequest.TransitionTime. Value))) &&

               base.GenericEquals(SetCEVoltageRequest);

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

                   $"{Voltage} V",

                   VoltageError.HasValue
                       ? $" ±{VoltageError.Value.Value}%"
                       : "",

                   ProcessingDelay.HasValue
                       ? $" delay: {ProcessingDelay.Value.TotalMilliseconds} ms'"
                       : "",

                   TransitionTime.HasValue
                       ? $", transition time: {TransitionTime.Value.TotalMilliseconds} ms"
                       : ""

               );

        #endregion

    }

}
