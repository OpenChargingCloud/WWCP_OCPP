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
    /// The SetMonitoringBase request.
    /// </summary>
    public class SetMonitoringBaseRequest : ARequest<SetMonitoringBaseRequest>,
                                            IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/setMonitoringBaseRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The monitoring base to be set.
        /// </summary>
        [Mandatory]
        public MonitoringBase  MonitoringBase    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetMonitoringBase request.
        /// </summary>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="MonitoringBase">The monitoring base to be set.</param>
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
        public SetMonitoringBaseRequest(SourceRouting            Destination,
                                        MonitoringBase           MonitoringBase,

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
                   nameof(SetMonitoringBaseRequest)[..^7],

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

            this.MonitoringBase = MonitoringBase;

            unchecked
            {
                hashCode = this.MonitoringBase.GetHashCode() * 3 ^
                           base.               GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetMonitoringBaseRequest",
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
        //     },
        //     "MonitoringBaseEnumType": {
        //       "description": "Specify which monitoring base will be set\r\n",
        //       "javaType": "MonitoringBaseEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "All",
        //         "FactoryDefault",
        //         "HardWiredOnly"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "monitoringBase": {
        //       "$ref": "#/definitions/MonitoringBaseEnumType"
        //     }
        //   },
        //   "required": [
        //     "monitoringBase"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomSetMonitoringBaseRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetMonitoringBase request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetMonitoringBaseRequestParser">A delegate to parse custom SetMonitoringBase requests.</param>
        public static SetMonitoringBaseRequest Parse(JObject                                                 JSON,
                                                     Request_Id                                              RequestId,
                                                     SourceRouting                                       Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTime?                                               RequestTimestamp                       = null,
                                                     TimeSpan?                                               RequestTimeout                         = null,
                                                     EventTracking_Id?                                       EventTrackingId                        = null,
                                                     CustomJObjectParserDelegate<SetMonitoringBaseRequest>?  CustomSetMonitoringBaseRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setMonitoringBaseRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetMonitoringBaseRequestParser))
            {
                return setMonitoringBaseRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetMonitoringBase request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out SetMonitoringBaseRequest, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetMonitoringBase request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetMonitoringBaseRequest">The parsed SetMonitoringBase request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetMonitoringBaseRequestParser">A delegate to parse custom SetMonitoringBase requests.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out SetMonitoringBaseRequest?      SetMonitoringBaseRequest,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               RequestTimestamp                       = null,
                                       TimeSpan?                                               RequestTimeout                         = null,
                                       EventTracking_Id?                                       EventTrackingId                        = null,
                                       CustomJObjectParserDelegate<SetMonitoringBaseRequest>?  CustomSetMonitoringBaseRequestParser   = null)
        {

            try
            {

                SetMonitoringBaseRequest = null;

                #region MonitoringBase       [mandatory]

                if (!JSON.ParseMandatory("monitoringBase",
                                         "display message",
                                         OCPPv2_1.MonitoringBase.TryParse,
                                         out MonitoringBase MonitoringBase,
                                         out ErrorResponse))
                {
                    return false;
                }

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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetMonitoringBaseRequest = new SetMonitoringBaseRequest(

                                               Destination,
                                               MonitoringBase,

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

                if (CustomSetMonitoringBaseRequestParser is not null)
                    SetMonitoringBaseRequest = CustomSetMonitoringBaseRequestParser(JSON,
                                                                                    SetMonitoringBaseRequest);

                return true;

            }
            catch (Exception e)
            {
                SetMonitoringBaseRequest  = null;
                ErrorResponse             = "The given JSON representation of a SetMonitoringBase request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetMonitoringBaseRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetMonitoringBaseRequestSerializer">A delegate to serialize custom SetMonitoringBase requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetMonitoringBaseRequest>?  CustomSetMonitoringBaseRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("monitoringBase",   MonitoringBase.ToString()),


                           Signatures.Any()
                               ? new JProperty("signatures",       new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.    ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetMonitoringBaseRequestSerializer is not null
                       ? CustomSetMonitoringBaseRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetMonitoringBaseRequest1, SetMonitoringBaseRequest2)

        /// <summary>
        /// Compares two SetMonitoringBase requests for equality.
        /// </summary>
        /// <param name="SetMonitoringBaseRequest1">A SetMonitoringBase request.</param>
        /// <param name="SetMonitoringBaseRequest2">Another SetMonitoringBase request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetMonitoringBaseRequest? SetMonitoringBaseRequest1,
                                           SetMonitoringBaseRequest? SetMonitoringBaseRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetMonitoringBaseRequest1, SetMonitoringBaseRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetMonitoringBaseRequest1 is null || SetMonitoringBaseRequest2 is null)
                return false;

            return SetMonitoringBaseRequest1.Equals(SetMonitoringBaseRequest2);

        }

        #endregion

        #region Operator != (SetMonitoringBaseRequest1, SetMonitoringBaseRequest2)

        /// <summary>
        /// Compares two SetMonitoringBase requests for inequality.
        /// </summary>
        /// <param name="SetMonitoringBaseRequest1">A SetMonitoringBase request.</param>
        /// <param name="SetMonitoringBaseRequest2">Another SetMonitoringBase request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetMonitoringBaseRequest? SetMonitoringBaseRequest1,
                                           SetMonitoringBaseRequest? SetMonitoringBaseRequest2)

            => !(SetMonitoringBaseRequest1 == SetMonitoringBaseRequest2);

        #endregion

        #endregion

        #region IEquatable<SetMonitoringBaseRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetMonitoringBase requests for equality.
        /// </summary>
        /// <param name="Object">A SetMonitoringBase request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetMonitoringBaseRequest setMonitoringBaseRequest &&
                   Equals(setMonitoringBaseRequest);

        #endregion

        #region Equals(SetMonitoringBaseRequest)

        /// <summary>
        /// Compares two SetMonitoringBase requests for equality.
        /// </summary>
        /// <param name="SetMonitoringBaseRequest">A SetMonitoringBase request to compare with.</param>
        public override Boolean Equals(SetMonitoringBaseRequest? SetMonitoringBaseRequest)

            => SetMonitoringBaseRequest is not null &&

               MonitoringBase.Equals(SetMonitoringBaseRequest.MonitoringBase) &&

               base.   GenericEquals(SetMonitoringBaseRequest);

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

            => MonitoringBase.ToString();

        #endregion

    }

}
