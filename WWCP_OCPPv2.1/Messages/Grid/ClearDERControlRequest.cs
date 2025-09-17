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
    /// The ClearDERControl request.
    /// </summary>
    public class ClearDERControlRequest : ARequest<ClearDERControlRequest>,
                                          IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/clearDERControlRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// True:  Clearing default Distributed Energy Resource (DER) controls.
        /// False: Clearing scheduled Distributed Energy Resource (DER) controls.
        /// </summary>
        [Mandatory]
        public Boolean          IsDefault      { get; }

        /// <summary>
        /// The optional type of all Distributed Energy Resource (DER) control settings to be cleared.
        /// Not used when _controlId_ is provided.
        /// </summary>
        [Optional]
        public DERControlType?  ControlType    { get; }

        /// <summary>
        /// The optional identification of Distributed Energy Resource (DER) control setting to clear.
        /// When omitted all settings for _controlType_ are cleared.
        /// </summary>
        [Optional]
        public DERControl_Id?   ControlId      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearDERControl request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="IsDefault">True: Clearing default Distributed Energy Resource (DER) controls; False: Clearing scheduled Distributed Energy Resource (DER) controls.</param>
        /// <param name="ControlType">The optional type of all Distributed Energy Resource (DER) control settings to be cleared. Not used when _controlId_ is provided.</param>
        /// <param name="ControlId">The optional identification of Distributed Energy Resource (DER) control setting to be cleared. When omitted all settings for _controlType_ are cleared.</param>
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
        public ClearDERControlRequest(SourceRouting            Destination,
                                      Boolean                  IsDefault,
                                      DERControlType?          ControlType           = null,
                                      DERControl_Id?           ControlId             = null,

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
                   nameof(ClearDERControlRequest)[..^7],

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

            this.IsDefault    = IsDefault;
            this.ControlType  = ControlType;
            this.ControlId    = ControlId;

            unchecked
            {

                hashCode = this.IsDefault.   GetHashCode()       * 7 ^
                          (this.ControlType?.GetHashCode() ?? 0) * 5 ^
                          (this.ControlId?.  GetHashCode() ?? 0) * 3 ^
                           base.             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:ClearDERControlRequest",
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
        /// Parse the given JSON representation of a ClearDERControl request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearDERControlRequestParser">A delegate to parse custom ClearDERControl requests.</param>
        public static ClearDERControlRequest Parse(JObject                                               JSON,
                                                   Request_Id                                            RequestId,
                                                   SourceRouting                                         Destination,
                                                   NetworkPath                                           NetworkPath,
                                                   DateTimeOffset?                                       RequestTimestamp                     = null,
                                                   TimeSpan?                                             RequestTimeout                       = null,
                                                   EventTracking_Id?                                     EventTrackingId                      = null,
                                                   CustomJObjectParserDelegate<ClearDERControlRequest>?  CustomClearDERControlRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var clearDERControlRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomClearDERControlRequestParser))
            {
                return clearDERControlRequest;
            }

            throw new ArgumentException("The given JSON representation of a ClearDERControl request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out ClearDERControlRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a ClearDERControl request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClearDERControlRequest">The parsed ClearDERControl request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearDERControlRequestParser">A delegate to parse custom ClearDERControl requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out ClearDERControlRequest?      ClearDERControlRequest,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       DateTimeOffset?                                       RequestTimestamp                     = null,
                                       TimeSpan?                                             RequestTimeout                       = null,
                                       EventTracking_Id?                                     EventTrackingId                      = null,
                                       CustomJObjectParserDelegate<ClearDERControlRequest>?  CustomClearDERControlRequestParser   = null)
        {

            try
            {

                ClearDERControlRequest = null;

                #region IsDefault      [mandatory]

                if (!JSON.ParseMandatory("isDefault",
                                         "is default or scheduled DER controls",
                                         out Boolean IsDefault,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ControlType    [optional]

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

                #region ControlId      [optional]

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


                #region Signatures     [optional, OCPP_CSE]

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

                #region CustomData     [optional]

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


                ClearDERControlRequest = new ClearDERControlRequest(

                                             Destination,
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

                if (CustomClearDERControlRequestParser is not null)
                    ClearDERControlRequest = CustomClearDERControlRequestParser(JSON,
                                                                                ClearDERControlRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearDERControlRequest  = null;
                ErrorResponse           = "The given JSON representation of a ClearDERControl request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearDERControlRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearDERControlRequestSerializer">A delegate to serialize custom ClearDERControl requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                   IncludeJSONLDContext                     = false,
                              CustomJObjectSerializerDelegate<ClearDERControlRequest>?  CustomClearDERControlRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?               CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",      DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("isDefault",     IsDefault),

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

            return CustomClearDERControlRequestSerializer is not null
                       ? CustomClearDERControlRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearDERControlRequest1, ClearDERControlRequest2)

        /// <summary>
        /// Compares two ClearDERControl requests for equality.
        /// </summary>
        /// <param name="ClearDERControlRequest1">A ClearDERControl request.</param>
        /// <param name="ClearDERControlRequest2">Another ClearDERControl request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearDERControlRequest? ClearDERControlRequest1,
                                           ClearDERControlRequest? ClearDERControlRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearDERControlRequest1, ClearDERControlRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ClearDERControlRequest1 is null || ClearDERControlRequest2 is null)
                return false;

            return ClearDERControlRequest1.Equals(ClearDERControlRequest2);

        }

        #endregion

        #region Operator != (ClearDERControlRequest1, ClearDERControlRequest2)

        /// <summary>
        /// Compares two ClearDERControl requests for inequality.
        /// </summary>
        /// <param name="ClearDERControlRequest1">A ClearDERControl request.</param>
        /// <param name="ClearDERControlRequest2">Another ClearDERControl request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearDERControlRequest? ClearDERControlRequest1,
                                           ClearDERControlRequest? ClearDERControlRequest2)

            => !(ClearDERControlRequest1 == ClearDERControlRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearDERControlRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearDERControl requests for equality.
        /// </summary>
        /// <param name="Object">A ClearDERControl request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearDERControlRequest clearDERControlRequest &&
                   Equals(clearDERControlRequest);

        #endregion

        #region Equals(ClearDERControlRequest)

        /// <summary>
        /// Compares two ClearDERControl requests for equality.
        /// </summary>
        /// <param name="ClearDERControlRequest">A ClearDERControl request to compare with.</param>
        public override Boolean Equals(ClearDERControlRequest? ClearDERControlRequest)

            => ClearDERControlRequest is not null &&

               IsDefault.Equals(ClearDERControlRequest.IsDefault) &&

            ((!ControlType.HasValue && !ClearDERControlRequest.ControlType.HasValue) ||
               ControlType.HasValue &&  ClearDERControlRequest.ControlType.HasValue && ControlType.Value.Equals(ClearDERControlRequest.ControlType.Value)) &&

            ((!ControlId.  HasValue && !ClearDERControlRequest.ControlId.  HasValue) ||
               ControlId.  HasValue &&  ClearDERControlRequest.ControlId.  HasValue && ControlId.  Value.Equals(ClearDERControlRequest.ControlId.  Value)) &&

               base.     GenericEquals(ClearDERControlRequest);

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

                   IsDefault
                       ? "Clearing default DER controls"
                       : "Clearing scheduled DER controls",

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
