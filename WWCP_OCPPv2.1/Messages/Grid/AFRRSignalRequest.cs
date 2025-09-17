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
    /// The AFRRSignal (Automatic Frequency Restoration Reserve) request.
    /// </summary>
    public class AFRRSignalRequest : ARequest<AFRRSignalRequest>,
                                     IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/aFRRSignalRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The time when the AFRRSignal becomes active.
        /// </summary>
        [Mandatory]
        public DateTimeOffset  ActivationTimestamp    { get; }

        /// <summary>
        /// The value of the AFRRSignal in v2xSignalWattCurve. Usually between -1 and 1.
        /// </summary>
        [Mandatory]
        public AFRR_Signal     Signal                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new automatic frequency restoration reserve (AFRR) signal request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ActivationTimestamp">The time when the signal becomes active.</param>
        /// <param name="Signal">The value of the AFRRSignal in v2xSignalWattCurve. Usually between -1 and 1.</param>
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
        public AFRRSignalRequest(SourceRouting            Destination,
                                 DateTimeOffset           ActivationTimestamp,
                                 AFRR_Signal              Signal,

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
                   nameof(AFRRSignalRequest)[..^7],

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

            this.ActivationTimestamp  = ActivationTimestamp;
            this.Signal               = Signal;

            unchecked
            {

                hashCode = this.ActivationTimestamp.GetHashCode() * 5 ^
                           this.Signal.             GetHashCode() * 3 ^
                           base.                    GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:AFRRSignalRequest",
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
        //         "timestamp": {
        //             "description": "Time when signal becomes active.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "signal": {
        //             "description": "Value of signal in _v2xSignalWattCurve_. ",
        //             "type": "integer"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "timestamp",
        //         "signal"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an AFRRSignal request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomAFRRSignalRequestParser">A delegate to parse custom AFRRSignal requests.</param>
        public static AFRRSignalRequest Parse(JObject                                          JSON,
                                              Request_Id                                       RequestId,
                                              SourceRouting                                    Destination,
                                              NetworkPath                                      NetworkPath,
                                              DateTimeOffset?                                  RequestTimestamp                = null,
                                              TimeSpan?                                        RequestTimeout                  = null,
                                              EventTracking_Id?                                EventTrackingId                 = null,
                                              CustomJObjectParserDelegate<AFRRSignalRequest>?  CustomAFRRSignalRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var afrrSignalRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomAFRRSignalRequestParser))
            {
                return afrrSignalRequest;
            }

            throw new ArgumentException("The given JSON representation of an AFRRSignal request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out AFRRSignalRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an AFRRSignal request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="AFRRSignalRequest">The parsed AFRRSignal request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomAFRRSignalRequestParser">A delegate to parse custom AFRRSignal requests.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       Request_Id                                       RequestId,
                                       SourceRouting                                    Destination,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out AFRRSignalRequest?      AFRRSignalRequest,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       DateTimeOffset?                                  RequestTimestamp                = null,
                                       TimeSpan?                                        RequestTimeout                  = null,
                                       EventTracking_Id?                                EventTrackingId                 = null,
                                       CustomJObjectParserDelegate<AFRRSignalRequest>?  CustomAFRRSignalRequestParser   = null)
        {

            try
            {

                AFRRSignalRequest = null;

                #region ActivationTimestamp    [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "activation timestamp",
                                         out DateTime ActivationTimestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signal                 [mandatory]

                if (!JSON.ParseMandatory("signal",
                                         "AFRRSignal",
                                         AFRR_Signal.TryParse,
                                         out AFRR_Signal Signal,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures             [optional, OCPP_CSE]

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

                #region CustomData             [optional]

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


                AFRRSignalRequest = new AFRRSignalRequest(

                                        Destination,
                                        ActivationTimestamp,
                                        Signal,

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

                if (CustomAFRRSignalRequestParser is not null)
                    AFRRSignalRequest = CustomAFRRSignalRequestParser(JSON,
                                                                      AFRRSignalRequest);

                return true;

            }
            catch (Exception e)
            {
                AFRRSignalRequest  = null;
                ErrorResponse      = "The given JSON representation of an AFRRSignal request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAFRRSignalRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAFRRSignalRequestSerializer">A delegate to serialize custom AFRRSignal requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                              IncludeJSONLDContext                = false,
                              CustomJObjectSerializerDelegate<AFRRSignalRequest>?  CustomAFRRSignalRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("timestamp",    ActivationTimestamp. ToISO8601()),

                                 new JProperty("signal",       Signal.              Value),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAFRRSignalRequestSerializer is not null
                       ? CustomAFRRSignalRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AFRRSignalRequest1, AFRRSignalRequest2)

        /// <summary>
        /// Compares two AFRRSignal requests for equality.
        /// </summary>
        /// <param name="AFRRSignalRequest1">An AFRRSignal request.</param>
        /// <param name="AFRRSignalRequest2">Another AFRRSignal request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AFRRSignalRequest? AFRRSignalRequest1,
                                           AFRRSignalRequest? AFRRSignalRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AFRRSignalRequest1, AFRRSignalRequest2))
                return true;

            // If one is null, but not both, return false.
            if (AFRRSignalRequest1 is null || AFRRSignalRequest2 is null)
                return false;

            return AFRRSignalRequest1.Equals(AFRRSignalRequest2);

        }

        #endregion

        #region Operator != (AFRRSignalRequest1, AFRRSignalRequest2)

        /// <summary>
        /// Compares two AFRRSignal requests for inequality.
        /// </summary>
        /// <param name="AFRRSignalRequest1">An AFRRSignal request.</param>
        /// <param name="AFRRSignalRequest2">Another AFRRSignal request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AFRRSignalRequest? AFRRSignalRequest1,
                                           AFRRSignalRequest? AFRRSignalRequest2)

            => !(AFRRSignalRequest1 == AFRRSignalRequest2);

        #endregion

        #endregion

        #region IEquatable<AFRRSignalRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AFRRSignal requests for equality.
        /// </summary>
        /// <param name="Object">An AFRRSignal request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AFRRSignalRequest afrrSignalRequest &&
                   Equals(afrrSignalRequest);

        #endregion

        #region Equals(AFRRSignalRequest)

        /// <summary>
        /// Compares two AFRRSignal requests for equality.
        /// </summary>
        /// <param name="AFRRSignalRequest">An AFRRSignal request to compare with.</param>
        public override Boolean Equals(AFRRSignalRequest? AFRRSignalRequest)

            => AFRRSignalRequest is not null &&

               ActivationTimestamp.Equals(AFRRSignalRequest.ActivationTimestamp) &&
               Signal.             Equals(AFRRSignalRequest.Signal)              &&

               base.        GenericEquals(AFRRSignalRequest);

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

            => $"'{Signal}' @ '{ActivationTimestamp}'";

        #endregion

    }

}
