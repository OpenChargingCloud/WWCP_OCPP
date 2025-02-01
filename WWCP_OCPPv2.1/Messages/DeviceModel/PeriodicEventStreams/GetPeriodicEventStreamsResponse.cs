/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/GetChargingCloud/WWCP_OCPP>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A get periodic event streams response.
    /// </summary>
    public class GetPeriodicEventStreamsResponse : AResponse<GetPeriodicEventStreamsRequest,
                                                             GetPeriodicEventStreamsResponse>,
                                                   IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getPeriodicEventStreamResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The response status.
        /// </summary>
        [Optional]
        public GenericStatus?                   Status                { get; }

        /// <summary>
        /// An optional element providing more information about the response status.
        /// </summary>
        [Optional]
        public StatusInfo?                      StatusInfo            { get; }

        /// <summary>
        /// The optional enumeration of the constant parts of periodic event streams.
        /// </summary>
        [Optional]
        public IEnumerable<ConstantStreamData>  ConstantStreamData    { get; }

        #endregion

        #region Constructor(s)

        #region GetPeriodicEventStreamResponse(Request, Status = null, StatusInfo = null, ...)

        /// <summary>
        /// Create a new open periodic event stream response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="Status">An optional response status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the response status.</param>
        /// <param name="ConstantStreamData">An optional enumeration of the constant parts of periodic event streams.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public GetPeriodicEventStreamsResponse(GetPeriodicEventStreamsRequest    Request,
                                               GenericStatus?                    Status               = null,
                                               StatusInfo?                       StatusInfo           = null,
                                               IEnumerable<ConstantStreamData>?  ConstantStreamData   = null,
                                               DateTime?                         ResponseTimestamp    = null,

                                               IEnumerable<KeyPair>?             SignKeys             = null,
                                               IEnumerable<SignInfo>?            SignInfos            = null,
                                               IEnumerable<Signature>?           Signatures           = null,

                                               CustomData?                       CustomData           = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status              = Status;
            this.StatusInfo          = StatusInfo;
            this.ConstantStreamData  = ConstantStreamData?.Distinct() ?? [];


            unchecked
            {

                hashCode = (this.Status?.           GetHashCode()  ?? 0) * 7 ^
                           (this.StatusInfo?.       GetHashCode()  ?? 0) * 5 ^
                            this.ConstantStreamData.CalcHashCode()       * 3 ^
                            base.                   GetHashCode();

            }

        }

        #endregion

        #region GetPeriodicEventStreamResponse(Request, Result)

        /// <summary>
        /// Create a new open periodic event stream response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public GetPeriodicEventStreamsResponse(GetPeriodicEventStreamsRequest  Request,
                                               Result                          Result)

            : base(Request,
                   Result)

        {

            this.ConstantStreamData  = [];
            this.Status              = GenericStatus.Rejected;

        }

        #endregion

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:GetPeriodicEventStreamResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "ConstantStreamDataType": {
        //             "javaType": "ConstantStreamData",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "id": {
        //                     "description": "Uniquely identifies the stream\r\n",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "params": {
        //                     "$ref": "#/definitions/PeriodicEventStreamParamsType"
        //                 },
        //                 "variableMonitoringId": {
        //                     "description": "Id of monitor used to report his event. It can be a preconfigured or hardwired monitor.\r\n",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id",
        //                 "variableMonitoringId",
        //                 "params"
        //             ]
        //         },
        //         "PeriodicEventStreamParamsType": {
        //             "javaType": "PeriodicEventStreamParams",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "interval": {
        //                     "description": "Time in seconds after which stream data is sent.\r\n",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "values": {
        //                     "description": "Number of items to be sent together in stream.\r\n",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
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
        //         "constantStreamData": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/ConstantStreamDataType"
        //             },
        //             "minItems": 1
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetPeriodicEventStreamResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get periodic event streams response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetPeriodicEventStreamResponseParser">A delegate to parse custom open periodic event stream responses.</param>
        public static GetPeriodicEventStreamsResponse Parse(GetPeriodicEventStreamsRequest                                 Request,
                                                            JObject                                                        JSON,
                                                            CustomJObjectParserDelegate<GetPeriodicEventStreamsResponse>?  CustomGetPeriodicEventStreamResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var getPeriodicEventStreamResponse,
                         out var errorResponse,
                         CustomGetPeriodicEventStreamResponseParser))
            {
                return getPeriodicEventStreamResponse;
            }

            throw new ArgumentException("The given JSON representation of a get periodic event streams response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetPeriodicEventStreamResponse, out ErrorResponse, CustomGetPeriodicEventStreamResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get periodic event streams response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetPeriodicEventStreamResponse">The parsed open periodic event stream response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetPeriodicEventStreamResponseParser">A delegate to parse custom open periodic event stream responses.</param>
        public static Boolean TryParse(GetPeriodicEventStreamsRequest                                 Request,
                                       JObject                                                        JSON,
                                       [NotNullWhen(true)]  out GetPeriodicEventStreamsResponse?      GetPeriodicEventStreamResponse,
                                       [NotNullWhen(false)] out String?                               ErrorResponse,
                                       CustomJObjectParserDelegate<GetPeriodicEventStreamsResponse>?  CustomGetPeriodicEventStreamResponseParser   = null)
        {

            try
            {

                GetPeriodicEventStreamResponse = null;

                #region Status                [optional]

                if (JSON.ParseOptional("status",
                                       "response status",
                                       GenericStatusExtensions.TryParse,
                                       out GenericStatus? Status,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StatusInfo            [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ConstantStreamData    [optional]

                if (JSON.ParseOptionalHashSet("constantStreamData",
                                              "status info",
                                              OCPPv2_1.ConstantStreamData.TryParse,
                                              out HashSet<ConstantStreamData> ConstantStreamData,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures            [optional, OCPP_CSE]

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

                #region CustomData            [optional]

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


                GetPeriodicEventStreamResponse = new GetPeriodicEventStreamsResponse(
                                                     Request,
                                                     Status,
                                                     StatusInfo,
                                                     ConstantStreamData,
                                                     null,
                                                     null,
                                                     null,
                                                     Signatures,
                                                     CustomData
                                                 );

                if (CustomGetPeriodicEventStreamResponseParser is not null)
                    GetPeriodicEventStreamResponse = CustomGetPeriodicEventStreamResponseParser(JSON,
                                                                                                GetPeriodicEventStreamResponse);

                return true;

            }
            catch (Exception e)
            {
                GetPeriodicEventStreamResponse  = null;
                ErrorResponse                   = "The given JSON representation of a GetPeriodicEventStreamsResponse is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetPeriodicEventStreamResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetPeriodicEventStreamResponseSerializer">A delegate to serialize custom open periodic event stream responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                            IncludeJSONLDContext                             = false,
                              CustomJObjectSerializerDelegate<GetPeriodicEventStreamsResponse>?  CustomGetPeriodicEventStreamResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                       CustomStatusInfoSerializer                       = null,
                              CustomJObjectSerializerDelegate<Signature>?                        CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",      DefaultJSONLDContext.ToString())
                               : null,

                           Status.HasValue
                               ? new JProperty("status",        Status.        Value.AsText())
                               : null,

                           StatusInfo is not null
                               ? new JProperty("statusInfo",    StatusInfo.          ToJSON(CustomStatusInfoSerializer,
                                                                                            CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetPeriodicEventStreamResponseSerializer is not null
                       ? CustomGetPeriodicEventStreamResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The open periodic event stream failed.
        /// </summary>
        public static GetPeriodicEventStreamsResponse Failed(GetPeriodicEventStreamsRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetPeriodicEventStreamResponse1, GetPeriodicEventStreamResponse2)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="GetPeriodicEventStreamResponse1">A get periodic event streams response.</param>
        /// <param name="GetPeriodicEventStreamResponse2">Another open periodic event stream response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetPeriodicEventStreamsResponse? GetPeriodicEventStreamResponse1,
                                           GetPeriodicEventStreamsResponse? GetPeriodicEventStreamResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetPeriodicEventStreamResponse1, GetPeriodicEventStreamResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetPeriodicEventStreamResponse1 is null || GetPeriodicEventStreamResponse2 is null)
                return false;

            return GetPeriodicEventStreamResponse1.Equals(GetPeriodicEventStreamResponse2);

        }

        #endregion

        #region Operator != (GetPeriodicEventStreamResponse1, GetPeriodicEventStreamResponse2)

        /// <summary>
        /// Compares two open periodic event stream responses for inequality.
        /// </summary>
        /// <param name="GetPeriodicEventStreamResponse1">A get periodic event streams response.</param>
        /// <param name="GetPeriodicEventStreamResponse2">Another open periodic event stream response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetPeriodicEventStreamsResponse? GetPeriodicEventStreamResponse1,
                                           GetPeriodicEventStreamsResponse? GetPeriodicEventStreamResponse2)

            => !(GetPeriodicEventStreamResponse1 == GetPeriodicEventStreamResponse2);

        #endregion

        #endregion

        #region IEquatable<GetPeriodicEventStreamResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="Object">A get periodic event streams response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetPeriodicEventStreamsResponse getPeriodicEventStreamResponse &&
                   Equals(getPeriodicEventStreamResponse);

        #endregion

        #region Equals(GetPeriodicEventStreamResponse)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="GetPeriodicEventStreamResponse">A get periodic event streams response to compare with.</param>
        public override Boolean Equals(GetPeriodicEventStreamsResponse? GetPeriodicEventStreamResponse)

            => GetPeriodicEventStreamResponse is not null &&

               Status.Equals(GetPeriodicEventStreamResponse.Status) &&

             ((StatusInfo is     null && GetPeriodicEventStreamResponse.StatusInfo is     null) ||
              (StatusInfo is not null && GetPeriodicEventStreamResponse.StatusInfo is not null && StatusInfo.Equals(GetPeriodicEventStreamResponse.StatusInfo))) &&

               base.GenericEquals(GetPeriodicEventStreamResponse);

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

                   Status.HasValue
                       ? Status.Value.AsText()
                       : "",

                   StatusInfo is not null
                       ? $", {StatusInfo}"
                       : "",

                   ConstantStreamData.Any()
                       ? $", {ConstantStreamData.AggregateWith(", ")}"
                       : ""

               );

        #endregion


    }

}
