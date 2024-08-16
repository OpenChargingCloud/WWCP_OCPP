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
    /// The GetBaseReport request.
    /// </summary>
    public class GetBaseReportRequest : ARequest<GetBaseReportRequest>,
                                        IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getBaseReportRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of the GetBaseReport request.
        /// </summary>
        [Mandatory]
        public Int64          GetBaseReportRequestId    { get; }

        /// <summary>
        /// The requested reporting base.
        /// </summary>
        [Mandatory]
        public ReportBase     ReportBase                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a GetBaseReport request.
        /// </summary>
        /// <param name="SourceRouting">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="GetBaseReportRequestId">An unique identification of the GetBaseReport request.</param>
        /// <param name="ReportBase">The requested reporting base.</param>
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
        public GetBaseReportRequest(SourceRouting            Destination,
                                    Int64                    GetBaseReportRequestId,
                                    ReportBase               ReportBase,

                                    IEnumerable<KeyPair>?    SignKeys            = null,
                                    IEnumerable<SignInfo>?   SignInfos           = null,
                                    IEnumerable<Signature>?  Signatures          = null,

                                    CustomData?              CustomData          = null,

                                    Request_Id?              RequestId           = null,
                                    DateTime?                RequestTimestamp    = null,
                                    TimeSpan?                RequestTimeout      = null,
                                    EventTracking_Id?        EventTrackingId     = null,
                                    NetworkPath?             NetworkPath         = null,
                                    CancellationToken        CancellationToken   = default)

            : base(Destination,
                   nameof(GetBaseReportRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            this.GetBaseReportRequestId  = GetBaseReportRequestId;
            this.ReportBase              = ReportBase;

            unchecked
            {
                hashCode = this.GetBaseReportRequestId.GetHashCode() * 5 ^
                           this.ReportBase.            GetHashCode() * 3 ^
                           base.                       GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetBaseReportRequest",
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
        //     "ReportBaseEnumType": {
        //       "description": "This field specifies the report base.",
        //       "javaType": "ReportBaseEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "ConfigurationInventory",
        //         "FullInventory",
        //         "SummaryInventory"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "requestId": {
        //       "description": "The Id of the request.",
        //       "type": "integer"
        //     },
        //     "reportBase": {
        //       "$ref": "#/definitions/ReportBaseEnumType"
        //     }
        //   },
        //   "required": [
        //     "requestId",
        //     "reportBase"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomGetBaseReportRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetBaseReport request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="SourceRouting">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetBaseReportRequestParser">A delegate to parse custom GetBaseReport requests.</param>
        public static GetBaseReportRequest Parse(JObject                                             JSON,
                                                 Request_Id                                          RequestId,
                                                 SourceRouting                                       SourceRouting,
                                                 NetworkPath                                         NetworkPath,
                                                 DateTime?                                           RequestTimestamp                   = null,
                                                 TimeSpan?                                           RequestTimeout                     = null,
                                                 EventTracking_Id?                                   EventTrackingId                    = null,
                                                 CustomJObjectParserDelegate<GetBaseReportRequest>?  CustomGetBaseReportRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                             SourceRouting,
                         NetworkPath,
                         out var getBaseReportRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetBaseReportRequestParser))
            {
                return getBaseReportRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetBaseReport request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out GetBaseReportRequest, out ErrorResponse, CustomRemoteStartTransactionRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetBaseReport request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="SourceRouting">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetBaseReportRequest">The parsed GetBaseReport request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetBaseReportRequestParser">A delegate to parse custom GetBaseReport requests.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       Request_Id                                          RequestId,
                                       SourceRouting                                       SourceRouting,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out GetBaseReportRequest?      GetBaseReportRequest,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       DateTime?                                           RequestTimestamp                   = null,
                                       TimeSpan?                                           RequestTimeout                     = null,
                                       EventTracking_Id?                                   EventTrackingId                    = null,
                                       CustomJObjectParserDelegate<GetBaseReportRequest>?  CustomGetBaseReportRequestParser   = null)
        {

            try
            {

                GetBaseReportRequest = null;

                #region GetBaseReportRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "certificate chain",
                                         out Int64 GetBaseReportRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ReportBase                [mandatory]

                if (!JSON.ParseMandatory("reportBase",
                                         "report base",
                                         OCPPv2_1.ReportBase.TryParse,
                                         out ReportBase ReportBase,
                                         out ErrorResponse))
                {
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetBaseReportRequest = new GetBaseReportRequest(

                                               SourceRouting,
                                           GetBaseReportRequestId,
                                           ReportBase,

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

                if (CustomGetBaseReportRequestParser is not null)
                    GetBaseReportRequest = CustomGetBaseReportRequestParser(JSON,
                                                                            GetBaseReportRequest);

                return true;

            }
            catch (Exception e)
            {
                GetBaseReportRequest  = null;
                ErrorResponse         = "The given JSON representation of a GetBaseReport request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetBaseReportRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetBaseReportRequestSerializer">A delegate to serialize custom GetBaseReport requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetBaseReportRequest>?  CustomGetBaseReportRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestId",    GetBaseReportRequestId),
                                 new JProperty("reportBase",   ReportBase.ToString()),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetBaseReportRequestSerializer is not null
                       ? CustomGetBaseReportRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetBaseReportRequest1, GetBaseReportRequest2)

        /// <summary>
        /// Compares two GetBaseReport requests for equality.
        /// </summary>
        /// <param name="GetBaseReportRequest1">A GetBaseReport request.</param>
        /// <param name="GetBaseReportRequest2">Another GetBaseReport request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetBaseReportRequest? GetBaseReportRequest1,
                                           GetBaseReportRequest? GetBaseReportRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetBaseReportRequest1, GetBaseReportRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetBaseReportRequest1 is null || GetBaseReportRequest2 is null)
                return false;

            return GetBaseReportRequest1.Equals(GetBaseReportRequest2);

        }

        #endregion

        #region Operator != (GetBaseReportRequest1, GetBaseReportRequest2)

        /// <summary>
        /// Compares two GetBaseReport requests for inequality.
        /// </summary>
        /// <param name="GetBaseReportRequest1">A GetBaseReport request.</param>
        /// <param name="GetBaseReportRequest2">Another GetBaseReport request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetBaseReportRequest? GetBaseReportRequest1,
                                           GetBaseReportRequest? GetBaseReportRequest2)

            => !(GetBaseReportRequest1 == GetBaseReportRequest2);

        #endregion

        #endregion

        #region IEquatable<GetBaseReportRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetBaseReport requests for equality.
        /// </summary>
        /// <param name="Object">A GetBaseReport request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetBaseReportRequest getBaseReportRequest &&
                   Equals(getBaseReportRequest);

        #endregion

        #region Equals(GetBaseReportRequest)

        /// <summary>
        /// Compares two GetBaseReport requests for equality.
        /// </summary>
        /// <param name="GetBaseReportRequest">A GetBaseReport request to compare with.</param>
        public override Boolean Equals(GetBaseReportRequest? GetBaseReportRequest)

            => GetBaseReportRequest is not null &&

               GetBaseReportRequestId.Equals(GetBaseReportRequest.GetBaseReportRequestId) &&
               ReportBase.            Equals(GetBaseReportRequest.ReportBase)             &&

               base.           GenericEquals(GetBaseReportRequest);

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

            => $"{ReportBase} / {GetBaseReportRequestId}";

        #endregion

    }

}
