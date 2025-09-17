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
    /// The AttachCable request.
    /// </summary>
    public class AttachCableRequest : ARequest<AttachCableRequest>,
                                      IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/attachCableRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The EVSE identification, when the charging station has more than one EVSE
        /// (0 > EVSEId ≤ MaxEVSEId).
        /// </summary>
        [Mandatory]
        public EVSE_Id        EVSEId           { get; }

        /// <summary>
        /// The optional connector identification, when the charging station has more than one connector on the given EVSE
        /// (0 > ConnectorId ≤ MaxConnectorId(EVSEId)). Default: 1
        /// </summary>
        [Optional]
        public Connector_Id?  ConnectorId      { get; }

        /// <summary>
        /// The resistor value to indicate the cable's maximum permissible current.
        /// </summary>
        [Mandatory]
        public Ohm            ResistorValue    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new AttachCable request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="EVSEId">The EVSE identification, when the charging station has more than one EVSE (0 > EVSEId ≤ MaxEVSEId).</param>
        /// <param name="ConnectorId">The optional connector identification, when the charging station has more than one connector on the given EVSE (0 > ConnectorId ≤ MaxConnectorId(EVSEId)). Default: 1</param>
        /// <param name="ResistorValue">The resistor value to indicate the cable's maximum permissible current.</param>
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
        public AttachCableRequest(SourceRouting            Destination,
                                  EVSE_Id                  EVSEId,
                                  Ohm                      ResistorValue,
                                  Connector_Id?            ConnectorId           = null,

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
                   nameof(AttachCableRequest)[..^7],

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

            this.EVSEId         = EVSEId;
            this.ConnectorId    = ConnectorId;
            this.ResistorValue  = ResistorValue;

            unchecked
            {

                hashCode = this.EVSEId.       GetHashCode()       * 7 ^
                          (this.ConnectorId?. GetHashCode() ?? 0) * 5 ^
                           this.ResistorValue.GetHashCode()       * 3 ^
                           base.              GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an AttachCable request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomAttachCableRequestParser">An optional delegate to parse custom AttachCable requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static AttachCableRequest Parse(JObject                                           JSON,
                                               Request_Id                                        RequestId,
                                               SourceRouting                                     Destination,
                                               NetworkPath                                       NetworkPath,
                                               DateTimeOffset?                                   RequestTimestamp                 = null,
                                               TimeSpan?                                         RequestTimeout                   = null,
                                               EventTracking_Id?                                 EventTrackingId                  = null,
                                               CustomJObjectParserDelegate<AttachCableRequest>?  CustomAttachCableRequestParser   = null,
                                               CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                               CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var attachCableRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomAttachCableRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return attachCableRequest;
            }

            throw new ArgumentException("The given JSON representation of an AttachCable request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out AttachCableRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an AttachCable request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="AttachCableRequest">The parsed AttachCable request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAttachCableRequestParser">An optional delegate to parse custom AttachCable requests.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       Request_Id                                        RequestId,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out AttachCableRequest?      AttachCableRequest,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTimeOffset?                                   RequestTimestamp                 = null,
                                       TimeSpan?                                         RequestTimeout                   = null,
                                       EventTracking_Id?                                 EventTrackingId                  = null,
                                       CustomJObjectParserDelegate<AttachCableRequest>?  CustomAttachCableRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                       CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            try
            {

                AttachCableRequest = null;

                #region EVSEId           [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id evseId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ResistorValue    [mandatory]

                if (!JSON.ParseMandatory("resistorValue",
                                         "resistor value",
                                         Ohm.TryParse,
                                         out Ohm resistorValue,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId      [optional]

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

                #region Signatures       [optional, OCPP_CSE]

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

                #region CustomData       [optional]

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


                AttachCableRequest = new AttachCableRequest(

                                         Destination,
                                         evseId,
                                         resistorValue,
                                         connectorId,

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

                if (CustomAttachCableRequestParser is not null)
                    AttachCableRequest = CustomAttachCableRequestParser(JSON,
                                                                        AttachCableRequest);

                return true;

            }
            catch (Exception e)
            {
                AttachCableRequest  = null;
                ErrorResponse       = "The given JSON representation of an AttachCable request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAttachCableRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAttachCableRequestSerializer">A delegate to serialize custom AttachCable requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AttachCableRequest>?  CustomAttachCableRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?           CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("evseId",          EVSEId.           Value),

                           ConnectorId.HasValue
                               ? new JProperty("connectorId",     ConnectorId.Value.Value)
                               : null,

                                 new JProperty("resistorValue",   ResistorValue.    Value),

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAttachCableRequestSerializer is not null
                       ? CustomAttachCableRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AttachCableRequest1, AttachCableRequest2)

        /// <summary>
        /// Compares two AttachCable requests for equality.
        /// </summary>
        /// <param name="AttachCableRequest1">An AttachCable request.</param>
        /// <param name="AttachCableRequest2">Another AttachCable request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AttachCableRequest? AttachCableRequest1,
                                           AttachCableRequest? AttachCableRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AttachCableRequest1, AttachCableRequest2))
                return true;

            // If one is null, but not both, return false.
            if (AttachCableRequest1 is null || AttachCableRequest2 is null)
                return false;

            return AttachCableRequest1.Equals(AttachCableRequest2);

        }

        #endregion

        #region Operator != (AttachCableRequest1, AttachCableRequest2)

        /// <summary>
        /// Compares two AttachCable requests for inequality.
        /// </summary>
        /// <param name="AttachCableRequest1">An AttachCable request.</param>
        /// <param name="AttachCableRequest2">Another AttachCable request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AttachCableRequest? AttachCableRequest1,
                                           AttachCableRequest? AttachCableRequest2)

            => !(AttachCableRequest1 == AttachCableRequest2);

        #endregion

        #endregion

        #region IEquatable<AttachCableRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AttachCable requests for equality.
        /// </summary>
        /// <param name="Object">An AttachCable request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AttachCableRequest attachCableRequest &&
                   Equals(attachCableRequest);

        #endregion

        #region Equals(AttachCableRequest)

        /// <summary>
        /// Compares two AttachCable requests for equality.
        /// </summary>
        /// <param name="AttachCableRequest">An AttachCable request to compare with.</param>
        public override Boolean Equals(AttachCableRequest? AttachCableRequest)

            => AttachCableRequest is not null &&

               EVSEId.       Equals(AttachCableRequest.EVSEId)        &&
               ResistorValue.Equals(AttachCableRequest.ResistorValue) &&

            ((!ConnectorId.HasValue && !AttachCableRequest.ConnectorId.HasValue) ||
             ( ConnectorId.HasValue &&  AttachCableRequest.ConnectorId.HasValue && ConnectorId.Value.Equals(AttachCableRequest.ConnectorId.Value))) &&

               base.GenericEquals(AttachCableRequest);

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

                   $"EVSE '{EVSEId}'",

                   ConnectorId.HasValue
                       ? $"/'{ConnectorId.Value}'"
                       : "",

                   $"{ResistorValue} Ω"

               );

        #endregion

    }

}
