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
    /// The SetMonitoringLevel request.
    /// </summary>
    public class SetMonitoringLevelRequest : ARequest<SetMonitoringLevelRequest>,
                                             IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/setMonitoringLevelRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The charging station SHALL only report events with a severity number lower than or equal to this severity.
        /// The severity range is 0-9, with 0 as the highest and 9 as the lowest severity level.
        /// </summary>
        [Mandatory]
        public Severities     Severity    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetMonitoringLevel request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Severity">The charging station SHALL only report events with a severity number lower than or equal to this severity.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SetMonitoringLevelRequest(SourceRouting            Destination,
                                         Severities               Severity,

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
                   nameof(SetMonitoringLevelRequest)[..^7],

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

            this.Severity = Severity;

            unchecked
            {
                hashCode = this.Severity.GetHashCode() * 3 ^
                           base.         GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:SetMonitoringLevelRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "severity": {
        //             "description": "The Charging Station SHALL only report events with a severity number lower than or equal to this severity.\r\nThe severity range is 0-9, with 0 as the highest and 9 as the lowest severity level.\r\n\r\nThe severity levels have the following meaning: +\r\n*0-Danger* +\r\nIndicates lives are potentially in danger. Urgent attention is needed and action should be taken immediately. +\r\n*1-Hardware Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to Hardware issues. Action is required. +\r\n*2-System Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to software or minor hardware issues. Action is required. +\r\n*3-Critical* +\r\nIndicates a critical error. Action is required. +\r\n*4-Error* +\r\nIndicates a non-urgent error. Action is required. +\r\n*5-Alert* +\r\nIndicates an alert event. Default severity for any type of monitoring event.  +\r\n*6-Warning* +\r\nIndicates a warning event. Action may be required. +\r\n*7-Notice* +\r\nIndicates an unusual event. No immediate action is required. +\r\n*8-Informational* +\r\nIndicates a regular operational event. May be used for reporting, measuring throughput, etc. No action is required. +\r\n*9-Debug* +\r\nIndicates information useful to developers for debugging, not useful during operations.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "severity"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SetMonitoringLevel request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetMonitoringLevelRequestParser">A delegate to parse custom SetMonitoringLevel requests.</param>
        public static SetMonitoringLevelRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      SourceRouting                                            Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                RequestTimestamp                        = null,
                                                      TimeSpan?                                                RequestTimeout                          = null,
                                                      EventTracking_Id?                                        EventTrackingId                         = null,
                                                      CustomJObjectParserDelegate<SetMonitoringLevelRequest>?  CustomSetMonitoringLevelRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setMonitoringLevelRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetMonitoringLevelRequestParser))
            {
                return setMonitoringLevelRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetMonitoringLevel request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SetMonitoringLevelRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SetMonitoringLevel request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetMonitoringLevelRequest">The parsed SetMonitoringLevel request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetMonitoringLevelRequestParser">A delegate to parse custom SetMonitoringLevel requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out SetMonitoringLevelRequest?      SetMonitoringLevelRequest,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                RequestTimestamp                        = null,
                                       TimeSpan?                                                RequestTimeout                          = null,
                                       EventTracking_Id?                                        EventTrackingId                         = null,
                                       CustomJObjectParserDelegate<SetMonitoringLevelRequest>?  CustomSetMonitoringLevelRequestParser   = null)
        {

            try
            {

                SetMonitoringLevelRequest = null;

                #region Severity             [mandatory]

                if (!JSON.ParseMandatory("severity",
                                         "severity",
                                         out Byte severity,
                                         out ErrorResponse))
                {
                    return false;
                }

                var Severity = SeveritiesExtensions.TryParse(severity);

                if (!Severity.HasValue)
                    return false;

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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetMonitoringLevelRequest = new SetMonitoringLevelRequest(

                                                Destination,
                                                Severity.Value,

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

                if (CustomSetMonitoringLevelRequestParser is not null)
                    SetMonitoringLevelRequest = CustomSetMonitoringLevelRequestParser(JSON,
                                                                                      SetMonitoringLevelRequest);

                return true;

            }
            catch (Exception e)
            {
                SetMonitoringLevelRequest  = null;
                ErrorResponse              = "The given JSON representation of a SetMonitoringLevel request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetMonitoringLevelRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetMonitoringLevelRequestSerializer">A delegate to serialize custom SetMonitoringLevel requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                      IncludeJSONLDContext                        = false,
                              CustomJObjectSerializerDelegate<SetMonitoringLevelRequest>?  CustomSetMonitoringLevelRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("severity",     Severity.            AsNumber()),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetMonitoringLevelRequestSerializer is not null
                       ? CustomSetMonitoringLevelRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetMonitoringLevelRequest1, SetMonitoringLevelRequest2)

        /// <summary>
        /// Compares two SetMonitoringLevel requests for equality.
        /// </summary>
        /// <param name="SetMonitoringLevelRequest1">A SetMonitoringLevel request.</param>
        /// <param name="SetMonitoringLevelRequest2">Another SetMonitoringLevel request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetMonitoringLevelRequest? SetMonitoringLevelRequest1,
                                           SetMonitoringLevelRequest? SetMonitoringLevelRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetMonitoringLevelRequest1, SetMonitoringLevelRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetMonitoringLevelRequest1 is null || SetMonitoringLevelRequest2 is null)
                return false;

            return SetMonitoringLevelRequest1.Equals(SetMonitoringLevelRequest2);

        }

        #endregion

        #region Operator != (SetMonitoringLevelRequest1, SetMonitoringLevelRequest2)

        /// <summary>
        /// Compares two SetMonitoringLevel requests for inequality.
        /// </summary>
        /// <param name="SetMonitoringLevelRequest1">A SetMonitoringLevel request.</param>
        /// <param name="SetMonitoringLevelRequest2">Another SetMonitoringLevel request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetMonitoringLevelRequest? SetMonitoringLevelRequest1,
                                           SetMonitoringLevelRequest? SetMonitoringLevelRequest2)

            => !(SetMonitoringLevelRequest1 == SetMonitoringLevelRequest2);

        #endregion

        #endregion

        #region IEquatable<SetMonitoringLevelRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetMonitoringLevel requests for equality.
        /// </summary>
        /// <param name="Object">A SetMonitoringLevel request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetMonitoringLevelRequest setMonitoringLevelRequest &&
                   Equals(setMonitoringLevelRequest);

        #endregion

        #region Equals(SetMonitoringLevelRequest)

        /// <summary>
        /// Compares two SetMonitoringLevel requests for equality.
        /// </summary>
        /// <param name="SetMonitoringLevelRequest">A SetMonitoringLevel request to compare with.</param>
        public override Boolean Equals(SetMonitoringLevelRequest? SetMonitoringLevelRequest)

            => SetMonitoringLevelRequest is not null &&

               Severity.   Equals(SetMonitoringLevelRequest.Severity) &&

               base.GenericEquals(SetMonitoringLevelRequest);

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

            => Severity.AsText();

        #endregion

    }

}
