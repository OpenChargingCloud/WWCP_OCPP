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
    /// The SetIDVoltage request.
    /// </summary>
    public class SetIDVoltageRequest : ARequest<SetIDVoltageRequest>,
                                       IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/setIDVoltageRequest");

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
        /// Create a new SetIDVoltage request.
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
        public SetIDVoltageRequest(SourceRouting            Destination,
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
                   nameof(SetIDVoltageRequest)[..^7],

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
        /// Parse the given JSON representation of a SetIDVoltage request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetIDVoltageRequestParser">An optional delegate to parse custom SetIDVoltage requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static SetIDVoltageRequest Parse(JObject                                            JSON,
                                                Request_Id                                         RequestId,
                                                SourceRouting                                      Destination,
                                                NetworkPath                                        NetworkPath,
                                                DateTime?                                          RequestTimestamp                  = null,
                                                TimeSpan?                                          RequestTimeout                    = null,
                                                EventTracking_Id?                                  EventTrackingId                   = null,
                                                CustomJObjectParserDelegate<SetIDVoltageRequest>?  CustomSetIDVoltageRequestParser   = null,
                                                CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                                CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setIDVoltageRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetIDVoltageRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setIDVoltageRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetIDVoltage request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SetIDVoltageRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SetIDVoltage request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetIDVoltageRequest">The parsed SetIDVoltage request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetIDVoltageRequestParser">An optional delegate to parse custom SetIDVoltage requests.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       Request_Id                                         RequestId,
                                       SourceRouting                                      Destination,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out SetIDVoltageRequest?      SetIDVoltageRequest,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTime?                                          RequestTimestamp                  = null,
                                       TimeSpan?                                          RequestTimeout                    = null,
                                       EventTracking_Id?                                  EventTrackingId                   = null,
                                       CustomJObjectParserDelegate<SetIDVoltageRequest>?  CustomSetIDVoltageRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                       CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            try
            {

                SetIDVoltageRequest = null;

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


                SetIDVoltageRequest = new SetIDVoltageRequest(

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

                if (CustomSetIDVoltageRequestParser is not null)
                    SetIDVoltageRequest = CustomSetIDVoltageRequestParser(JSON,
                                                                          SetIDVoltageRequest);

                return true;

            }
            catch (Exception e)
            {
                SetIDVoltageRequest  = null;
                ErrorResponse        = "The given JSON representation of a SetIDVoltage request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetIDVoltageRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetIDVoltageRequestSerializer">A delegate to serialize custom SetIDVoltage requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetIDVoltageRequest>?  CustomSetIDVoltageRequestSerializer   = null,
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

            return CustomSetIDVoltageRequestSerializer is not null
                       ? CustomSetIDVoltageRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetIDVoltageRequest1, SetIDVoltageRequest2)

        /// <summary>
        /// Compares two SetIDVoltage requests for equality.
        /// </summary>
        /// <param name="SetIDVoltageRequest1">A SetIDVoltage request.</param>
        /// <param name="SetIDVoltageRequest2">Another SetIDVoltage request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetIDVoltageRequest? SetIDVoltageRequest1,
                                           SetIDVoltageRequest? SetIDVoltageRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetIDVoltageRequest1, SetIDVoltageRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetIDVoltageRequest1 is null || SetIDVoltageRequest2 is null)
                return false;

            return SetIDVoltageRequest1.Equals(SetIDVoltageRequest2);

        }

        #endregion

        #region Operator != (SetIDVoltageRequest1, SetIDVoltageRequest2)

        /// <summary>
        /// Compares two SetIDVoltage requests for inequality.
        /// </summary>
        /// <param name="SetIDVoltageRequest1">A SetIDVoltage request.</param>
        /// <param name="SetIDVoltageRequest2">Another SetIDVoltage request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetIDVoltageRequest? SetIDVoltageRequest1,
                                           SetIDVoltageRequest? SetIDVoltageRequest2)

            => !(SetIDVoltageRequest1 == SetIDVoltageRequest2);

        #endregion

        #endregion

        #region IEquatable<SetIDVoltageRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetIDVoltage requests for equality.
        /// </summary>
        /// <param name="Object">A SetIDVoltage request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetIDVoltageRequest setIDVoltageRequest &&
                   Equals(setIDVoltageRequest);

        #endregion

        #region Equals(SetIDVoltageRequest)

        /// <summary>
        /// Compares two SetIDVoltage requests for equality.
        /// </summary>
        /// <param name="SetIDVoltageRequest">A SetIDVoltage request to compare with.</param>
        public override Boolean Equals(SetIDVoltageRequest? SetIDVoltageRequest)

            => SetIDVoltageRequest is not null &&

               Voltage.Equals(SetIDVoltageRequest.Voltage) &&

            ((!VoltageError.   HasValue && !SetIDVoltageRequest.VoltageError.   HasValue) ||
              (VoltageError.   HasValue &&  SetIDVoltageRequest.VoltageError.   HasValue && VoltageError.   Value.Equals(SetIDVoltageRequest.VoltageError.   Value))) &&

            ((!ProcessingDelay.HasValue && !SetIDVoltageRequest.ProcessingDelay.HasValue) ||
              (ProcessingDelay.HasValue &&  SetIDVoltageRequest.ProcessingDelay.HasValue && ProcessingDelay.Value.Equals(SetIDVoltageRequest.ProcessingDelay.Value))) &&

            ((!TransitionTime. HasValue && !SetIDVoltageRequest.TransitionTime. HasValue) ||
              (TransitionTime. HasValue &&  SetIDVoltageRequest.TransitionTime. HasValue && TransitionTime. Value.Equals(SetIDVoltageRequest.TransitionTime. Value))) &&

               base.GenericEquals(SetIDVoltageRequest);

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
