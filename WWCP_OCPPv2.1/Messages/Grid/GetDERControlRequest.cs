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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The GetDERControl request.
    /// </summary>
    public class GetDERControlRequest : ARequest<GetDERControlRequest>,
                                        IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getDERControlRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The requestId to be used in ReportDERControlRequest.
        /// </summary>
        [Mandatory]
        public Int32            GetDERControlRequestId    { get; }

        /// <summary>
        /// True:  Get a default Distributed Energy Resource (DER) control.
        /// False: Get a scheduled Distributed Energy Resource (DER) control.
        /// </summary>
        [Optional]
        public Boolean?         IsDefault                 { get; }

        /// <summary>
        /// The optional type of all Distributed Energy Resource (DER) control settings to be cleared.
        /// Not used when _controlId_ is provided.
        /// </summary>
        [Optional]
        public DERControlType?  ControlType               { get; }

        /// <summary>
        /// The optional identification of the Distributed Energy Resource (DER) control setting to get.
        /// When omitted all settings for _controlType_ are retrieved.
        /// </summary>
        [Optional]
        public DERControl_Id?   ControlId                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetDERControl request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="GetDERControlRequestId">The requestId to be used in ReportDERControlRequest.</param>
        /// <param name="IsDefault">True: Get a default Distributed Energy Resource (DER) control; False: Get scheduled Distributed Energy Resource (DER) control.</param>
        /// <param name="ControlType">The optional type of all Distributed Energy Resource (DER) control settings to be cleared. Not used when _controlId_ is provided.</param>
        /// <param name="ControlId">The optional identification of the Distributed Energy Resource (DER) control setting to get. When omitted all settings for _controlType_ are retrieved.</param>
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
        public GetDERControlRequest(SourceRouting            Destination,
                                    Int32                    GetDERControlRequestId,
                                    Boolean?                 IsDefault             = null,
                                    DERControlType?          ControlType           = null,
                                    DERControl_Id?           ControlId             = null,

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
                   nameof(GetDERControlRequest)[..^7],

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

            this.GetDERControlRequestId  = GetDERControlRequestId;
            this.IsDefault               = IsDefault;
            this.ControlType             = ControlType;
            this.ControlId               = ControlId;

            unchecked
            {

                hashCode = this.GetDERControlRequestId.GetHashCode()       * 11 ^
                          (this.IsDefault?.            GetHashCode() ?? 0) *  7 ^
                          (this.ControlType?.          GetHashCode() ?? 0) *  5 ^
                          (this.ControlId?.            GetHashCode() ?? 0) *  3 ^
                           base.                       GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:GetDERControlRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "DERControlEnumType": {
        //             "description": "Type of control settings to retrieve. Not used when _controlId_ is provided.",
        //             "javaType": "DERControlEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "EnterService",
        //                 "FreqDroop",
        //                 "FreqWatt",
        //                 "FixedPFAbsorb",
        //                 "FixedPFInject",
        //                 "FixedVar",
        //                 "Gradients",
        //                 "HFMustTrip",
        //                 "HFMayTrip",
        //                 "HVMustTrip",
        //                 "HVMomCess",
        //                 "HVMayTrip",
        //                 "LimitMaxDischarge",
        //                 "LFMustTrip",
        //                 "LVMustTrip",
        //                 "LVMomCess",
        //                 "LVMayTrip",
        //                 "PowerMonitoringMustTrip",
        //                 "VoltVar",
        //                 "VoltWatt",
        //                 "WattPF",
        //                 "WattVar"
        //             ]
        //         },
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
        //         "requestId": {
        //             "description": "RequestId to be used in ReportDERControlRequest.",
        //             "type": "integer"
        //         },
        //         "isDefault": {
        //             "description": "True: get a default DER control. False: get a scheduled control.",
        //             "type": "boolean"
        //         },
        //         "controlType": {
        //             "$ref": "#/definitions/DERControlEnumType"
        //         },
        //         "controlId": {
        //             "description": "Id of setting to get. When omitted all settings for _controlType_ are retrieved.",
        //             "type": "string",
        //             "maxLength": 36
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "requestId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetDERControl request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetDERControlRequestParser">A delegate to parse custom GetDERControl requests.</param>
        public static GetDERControlRequest Parse(JObject                                             JSON,
                                                 Request_Id                                          RequestId,
                                                 SourceRouting                                       Destination,
                                                 NetworkPath                                         NetworkPath,
                                                 DateTime?                                           RequestTimestamp                   = null,
                                                 TimeSpan?                                           RequestTimeout                     = null,
                                                 EventTracking_Id?                                   EventTrackingId                    = null,
                                                 CustomJObjectParserDelegate<GetDERControlRequest>?  CustomGetDERControlRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getDERControlRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetDERControlRequestParser))
            {
                return getDERControlRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetDERControl request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out GetDERControlRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetDERControl request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetDERControlRequest">The parsed GetDERControl request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetDERControlRequestParser">A delegate to parse custom GetDERControl requests.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       Request_Id                                          RequestId,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out GetDERControlRequest?      GetDERControlRequest,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       DateTime?                                           RequestTimestamp                   = null,
                                       TimeSpan?                                           RequestTimeout                     = null,
                                       EventTracking_Id?                                   EventTrackingId                    = null,
                                       CustomJObjectParserDelegate<GetDERControlRequest>?  CustomGetDERControlRequestParser   = null)
        {

            try
            {

                GetDERControlRequest = null;

                #region GetDERControlRequestId    [mandatory]

                if (!JSON.ParseMandatory("isDefault",
                                         "is default or scheduled DER controls",
                                         out Int32 GetDERControlRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IsDefault                 [optional]

                if (JSON.ParseOptional("isDefault",
                                       "is default or scheduled DER controls",
                                       out Boolean? IsDefault,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ControlType               [optional]

                if (JSON.ParseOptional("controlType",
                                       "control type",
                                       DERControlType.TryParse,
                                       out DERControlType? ControlType,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ControlId                 [optional]

                if (JSON.ParseOptional("controlId",
                                       "control identification",
                                       DERControl_Id.TryParse,
                                       out DERControl_Id? ControlId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures                [optional, OCPP_CSE]

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

                #region CustomData                [optional]

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


                GetDERControlRequest = new GetDERControlRequest(

                                           Destination,
                                           GetDERControlRequestId,
                                           IsDefault,
                                           ControlType,
                                           ControlId,

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

                if (CustomGetDERControlRequestParser is not null)
                    GetDERControlRequest = CustomGetDERControlRequestParser(JSON,
                                                                            GetDERControlRequest);

                return true;

            }
            catch (Exception e)
            {
                GetDERControlRequest  = null;
                ErrorResponse         = "The given JSON representation of a GetDERControl request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetDERControlRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetDERControlRequestSerializer">A delegate to serialize custom GetDERControl requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                 IncludeJSONLDContext                   = false,
                              CustomJObjectSerializerDelegate<GetDERControlRequest>?  CustomGetDERControlRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",      DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("requestId",     GetDERControlRequestId),

                           IsDefault.  HasValue
                               ? new JProperty("isDefault",     IsDefault.  Value)
                               : null,

                           ControlType.HasValue
                               ? new JProperty("controlType",   ControlType.Value.   ToString())
                               : null,

                           ControlId.  HasValue
                               ? new JProperty("controlId",     ControlId.  Value.   ToString())
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetDERControlRequestSerializer is not null
                       ? CustomGetDERControlRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetDERControlRequest1, GetDERControlRequest2)

        /// <summary>
        /// Compares two GetDERControl requests for equality.
        /// </summary>
        /// <param name="GetDERControlRequest1">A GetDERControl request.</param>
        /// <param name="GetDERControlRequest2">Another GetDERControl request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetDERControlRequest? GetDERControlRequest1,
                                           GetDERControlRequest? GetDERControlRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetDERControlRequest1, GetDERControlRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetDERControlRequest1 is null || GetDERControlRequest2 is null)
                return false;

            return GetDERControlRequest1.Equals(GetDERControlRequest2);

        }

        #endregion

        #region Operator != (GetDERControlRequest1, GetDERControlRequest2)

        /// <summary>
        /// Compares two GetDERControl requests for inequality.
        /// </summary>
        /// <param name="GetDERControlRequest1">A GetDERControl request.</param>
        /// <param name="GetDERControlRequest2">Another GetDERControl request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetDERControlRequest? GetDERControlRequest1,
                                           GetDERControlRequest? GetDERControlRequest2)

            => !(GetDERControlRequest1 == GetDERControlRequest2);

        #endregion

        #endregion

        #region IEquatable<GetDERControlRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetDERControl requests for equality.
        /// </summary>
        /// <param name="Object">A GetDERControl request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetDERControlRequest getDERControlRequest &&
                   Equals(getDERControlRequest);

        #endregion

        #region Equals(GetDERControlRequest)

        /// <summary>
        /// Compares two GetDERControl requests for equality.
        /// </summary>
        /// <param name="GetDERControlRequest">A GetDERControl request to compare with.</param>
        public override Boolean Equals(GetDERControlRequest? GetDERControlRequest)

            => GetDERControlRequest is not null &&

               GetDERControlRequestId.Equals(GetDERControlRequest.GetDERControlRequestId) &&

            ((!IsDefault.  HasValue && !GetDERControlRequest.IsDefault.  HasValue) ||
               IsDefault.  HasValue &&  GetDERControlRequest.IsDefault.  HasValue && IsDefault.  Value.Equals(GetDERControlRequest.IsDefault.  Value)) &&

            ((!ControlType.HasValue && !GetDERControlRequest.ControlType.HasValue) ||
               ControlType.HasValue &&  GetDERControlRequest.ControlType.HasValue && ControlType.Value.Equals(GetDERControlRequest.ControlType.Value)) &&

            ((!ControlId.  HasValue && !GetDERControlRequest.ControlId.  HasValue) ||
               ControlId.  HasValue &&  GetDERControlRequest.ControlId.  HasValue && ControlId.  Value.Equals(GetDERControlRequest.ControlId.  Value)) &&

               base.     GenericEquals(GetDERControlRequest);

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

                   $"{GetDERControlRequestId}: ",

                   IsDefault.HasValue
                       ? "get default DER controls"
                       : "get scheduled DER controls",

                   ControlType.HasValue
                       ? $", for control setting '{ControlType.Value}'"
                       : "",

                   ControlId.  HasValue
                       ? $", for control id: '{ControlId.Value}'"
                       : ""

                );

        #endregion

    }

}
