/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The ClearVariableMonitoring request.
    /// </summary>
    public class ClearVariableMonitoringRequest : ARequest<ClearVariableMonitoringRequest>,
                                                  IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/clearVariableMonitoringRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The enumeration of variable monitoring identifications to clear.
        /// </summary>
        [Mandatory]
        public IEnumerable<VariableMonitoring_Id>  VariableMonitoringIds    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearVariableMonitoring request.
        /// </summary>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="VariableMonitoringIds">An enumeration of variable monitoring identifications to clear.</param>
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
        public ClearVariableMonitoringRequest(SourceRouting                       Destination,
                                              IEnumerable<VariableMonitoring_Id>  VariableMonitoringIds,

                                              IEnumerable<KeyPair>?               SignKeys              = null,
                                              IEnumerable<SignInfo>?              SignInfos             = null,
                                              IEnumerable<Signature>?             Signatures            = null,

                                              CustomData?                         CustomData            = null,

                                              Request_Id?                         RequestId             = null,
                                              DateTime?                           RequestTimestamp      = null,
                                              TimeSpan?                           RequestTimeout        = null,
                                              EventTracking_Id?                   EventTrackingId       = null,
                                              NetworkPath?                        NetworkPath           = null,
                                              SerializationFormats?               SerializationFormat   = null,
                                              CancellationToken                   CancellationToken     = default)

            : base(Destination,
                   nameof(ClearVariableMonitoringRequest)[..^7],

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

            if (!VariableMonitoringIds.Any())
                throw new ArgumentException("The given enumeration of variable monitoring identifications must not be empty!",
                                            nameof(VariableMonitoringIds));

            this.VariableMonitoringIds  = VariableMonitoringIds.Distinct();

            unchecked
            {
                hashCode = this.VariableMonitoringIds.GetHashCode() * 3 ^
                           base.                      GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ClearVariableMonitoringRequest",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "id": {
        //       "description": "List of the monitors to be cleared, identified by there Id.",
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "type": "integer"
        //       },
        //       "minItems": 1
        //     }
        //   },
        //   "required": [
        //     "id"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomClearVariableMonitoringRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ClearVariableMonitoring request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearVariableMonitoringRequestParser">A delegate to parse custom ClearVariableMonitoring requests.</param>
        public static ClearVariableMonitoringRequest Parse(JObject                                                       JSON,
                                                           Request_Id                                                    RequestId,
                                                           SourceRouting                                             Destination,
                                                           NetworkPath                                                   NetworkPath,
                                                           DateTime?                                                     RequestTimestamp                             = null,
                                                           TimeSpan?                                                     RequestTimeout                               = null,
                                                           EventTracking_Id?                                             EventTrackingId                              = null,
                                                           CustomJObjectParserDelegate<ClearVariableMonitoringRequest>?  CustomClearVariableMonitoringRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var clearVariableMonitoringRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomClearVariableMonitoringRequestParser))
            {
                return clearVariableMonitoringRequest;
            }

            throw new ArgumentException("The given JSON representation of a ClearVariableMonitoring request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out ClearVariableMonitoringRequest, out ErrorResponse, CustomClearVariableMonitoringRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ClearVariableMonitoring request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClearVariableMonitoringRequest">The parsed ClearVariableMonitoring request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearVariableMonitoringRequestParser">A delegate to parse custom ClearVariableMonitoring requests.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       Request_Id                                                    RequestId,
                                       SourceRouting                                             Destination,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out ClearVariableMonitoringRequest?      ClearVariableMonitoringRequest,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       DateTime?                                                     RequestTimestamp                             = null,
                                       TimeSpan?                                                     RequestTimeout                               = null,
                                       EventTracking_Id?                                             EventTrackingId                              = null,
                                       CustomJObjectParserDelegate<ClearVariableMonitoringRequest>?  CustomClearVariableMonitoringRequestParser   = null)
        {

            try
            {

                ClearVariableMonitoringRequest = null;

                #region VariableMonitoringIds    [mandatory]

                if (!JSON.ParseMandatoryNumericHashSet("id",
                                                       "variable monitoring identification",
                                                       VariableMonitoring_Id.TryParse,
                                                       out HashSet<VariableMonitoring_Id> VariableMonitoringIds,
                                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures               [optional, OCPP_CSE]

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

                #region CustomData               [optional]

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


                ClearVariableMonitoringRequest = new ClearVariableMonitoringRequest(

                                                     Destination,
                                                     VariableMonitoringIds,

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

                if (CustomClearVariableMonitoringRequestParser is not null)
                    ClearVariableMonitoringRequest = CustomClearVariableMonitoringRequestParser(JSON,
                                                                                                ClearVariableMonitoringRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearVariableMonitoringRequest  = null;
                ErrorResponse                   = "The given JSON representation of a ClearVariableMonitoring request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearVariableMonitoringRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearVariableMonitoringRequestSerializer">A delegate to serialize custom ClearVariableMonitoring requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearVariableMonitoringRequest>?  CustomClearVariableMonitoringRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",           new JArray(VariableMonitoringIds.Select(variableMonitoringId => variableMonitoringId.Value))),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.           Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                     CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearVariableMonitoringRequestSerializer is not null
                       ? CustomClearVariableMonitoringRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearVariableMonitoringRequest1, ClearVariableMonitoringRequest2)

        /// <summary>
        /// Compares two ClearVariableMonitoring requests for equality.
        /// </summary>
        /// <param name="ClearVariableMonitoringRequest1">A ClearVariableMonitoring request.</param>
        /// <param name="ClearVariableMonitoringRequest2">Another ClearVariableMonitoring request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearVariableMonitoringRequest? ClearVariableMonitoringRequest1,
                                           ClearVariableMonitoringRequest? ClearVariableMonitoringRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearVariableMonitoringRequest1, ClearVariableMonitoringRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ClearVariableMonitoringRequest1 is null || ClearVariableMonitoringRequest2 is null)
                return false;

            return ClearVariableMonitoringRequest1.Equals(ClearVariableMonitoringRequest2);

        }

        #endregion

        #region Operator != (ClearVariableMonitoringRequest1, ClearVariableMonitoringRequest2)

        /// <summary>
        /// Compares two ClearVariableMonitoring requests for inequality.
        /// </summary>
        /// <param name="ClearVariableMonitoringRequest1">A ClearVariableMonitoring request.</param>
        /// <param name="ClearVariableMonitoringRequest2">Another ClearVariableMonitoring request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearVariableMonitoringRequest? ClearVariableMonitoringRequest1,
                                           ClearVariableMonitoringRequest? ClearVariableMonitoringRequest2)

            => !(ClearVariableMonitoringRequest1 == ClearVariableMonitoringRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearVariableMonitoringRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearVariableMonitoring requests for equality.
        /// </summary>
        /// <param name="Object">A ClearVariableMonitoring request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearVariableMonitoringRequest clearVariableMonitoringRequest &&
                   Equals(clearVariableMonitoringRequest);

        #endregion

        #region Equals(ClearVariableMonitoringRequest)

        /// <summary>
        /// Compares two ClearVariableMonitoring requests for equality.
        /// </summary>
        /// <param name="ClearVariableMonitoringRequest">A ClearVariableMonitoring request to compare with.</param>
        public override Boolean Equals(ClearVariableMonitoringRequest? ClearVariableMonitoringRequest)

            => ClearVariableMonitoringRequest is not null &&

               VariableMonitoringIds.Count().Equals(ClearVariableMonitoringRequest.VariableMonitoringIds.Count()) &&
               VariableMonitoringIds.All(id => ClearVariableMonitoringRequest.VariableMonitoringIds.Contains(id)) &&

               base.GenericEquals(ClearVariableMonitoringRequest);

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

            => VariableMonitoringIds.AggregateWith(", ");

        #endregion

    }

}
