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

using System.Xml.Linq;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A GetCompositeSchedule response.
    /// </summary>
    public class GetCompositeScheduleResponse : AResponse<GetCompositeScheduleRequest,
                                                          GetCompositeScheduleResponse>,
                                                IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/getCompositeScheduleResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext               Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The result of the request.
        /// </summary>
        public GetCompositeScheduleStatus  Status              { get; }

        /// <summary>
        /// The charging schedule contained in this notification
        /// applies to a specific connector.
        /// </summary>
        public Connector_Id?               ConnectorId         { get; }

        /// <summary>
        /// The periods contained in the charging profile are relative
        /// to this timestamp.
        /// </summary>
        public DateTime?                   ScheduleStart       { get; }

        /// <summary>
        /// The planned composite charging schedule, the energy consumption
        /// over time. Always relative to ScheduleStart.
        /// </summary>
        public ChargingSchedule?           ChargingSchedule    { get;  }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetCompositeSchedule response.
        /// </summary>
        /// <param name="Request">The GetCompositeSchedule request leading to this response.</param>
        /// <param name="Status">The success or failure of the GetCompositeSchedule command.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetCompositeScheduleResponse(GetCompositeScheduleRequest  Request,
                                            GetCompositeScheduleStatus   Status,
                                            Connector_Id?                ConnectorId           = null,
                                            DateTime?                    ScheduleStart         = null,
                                            ChargingSchedule?            ChargingSchedule      = null,

                                            Result?                      Result                = null,
                                            DateTime?                    ResponseTimestamp     = null,

                                            SourceRouting?               Destination           = null,
                                            NetworkPath?                 NetworkPath           = null,

                                            IEnumerable<KeyPair>?        SignKeys              = null,
                                            IEnumerable<SignInfo>?       SignInfos             = null,
                                            IEnumerable<Signature>?      Signatures            = null,

                                            CustomData?                  CustomData            = null,

                                            SerializationFormats?        SerializationFormat   = null,
                                            CancellationToken            CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.Status            = Status;
            this.ConnectorId       = ConnectorId;
            this.ScheduleStart     = ScheduleStart;
            this.ChargingSchedule  = ChargingSchedule;

            unchecked
            {

                hashCode = this.Status.           GetHashCode()       * 11 ^
                          (this.ConnectorId?.     GetHashCode() ?? 0) *  7 ^
                          (this.ScheduleStart?.   GetHashCode() ?? 0) *  5 ^
                          (this.ChargingSchedule?.GetHashCode() ?? 0) *  3 ^
                           base.                  GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:getCompositeScheduleResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //          <!--Optional:-->
        //          <ns:connectorId>?</ns:connectorId>
        //
        //          <!--Optional:-->
        //          <ns:scheduleStart>?</ns:scheduleStart>
        //
        //          <!--Optional:-->
        //          <ns:chargingSchedule>
        //
        //             <!--Optional:-->
        //             <ns:duration>?</ns:duration>
        //
        //             <!--Optional:-->
        //             <ns:startSchedule>?</ns:startSchedule>
        //
        //             <ns:chargingRateUnit>?</ns:chargingRateUnit>
        //
        //             <!--1 or more repetitions:-->
        //             <ns:chargingSchedulePeriod>
        //
        //                <ns:startPeriod>?</ns:startPeriod>
        //                <ns:limit>?</ns:limit>
        //
        //                <!--Optional:-->
        //                <ns:numberPhases>?</ns:numberPhases>
        //
        //             </ns:chargingSchedulePeriod>
        //
        //             <!--Optional:-->
        //             <ns:minChargingRate>?</ns:minChargingRate>
        //
        //          </ns:chargingSchedule>
        //
        //       </ns:getCompositeScheduleResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetCompositeScheduleResponse",
        //     "title":   "GetCompositeScheduleResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected"
        //             ]
        //         },
        //     "connectorId": {
        //         "type": "integer"
        //         },
        //     "scheduleStart": {
        //         "type": "string",
        //         "format": "date-time"
        //     },
        //     "chargingSchedule": {
        //         "type": "object",
        //         "properties": {
        //             "duration": {
        //                 "type": "integer"
        //             },
        //             "startSchedule": {
        //                 "type": "string",
        //                 "format": "date-time"
        //             },
        //             "chargingRateUnit": {
        //                 "type": "string",
        //                 "additionalProperties": false,
        //                 "enum": [
        //                     "A",
        //                     "W"
        //                     ]
        //             },
        //             "chargingSchedulePeriod": {
        //                 "type": "array",
        //                 "items": {
        //                     "type": "object",
        //                     "properties": {
        //                         "startPeriod": {
        //                             "type": "integer"
        //                         },
        //                         "limit": {
        //                             "type": "number",
        //                             "multipleOf" : 0.1
        //                         },
        //                         "numberPhases": {
        //                             "type": "integer"
        //                         }
        //                     },
        //                     "additionalProperties": false,
        //                     "required": [
        //                         "startPeriod",
        //                         "limit"
        //                         ]
        //                 }
        //             },
        //             "minChargingRate": {
        //                 "type": "number",
        //                 "multipleOf" : 0.1
        //             }
        //         },
        //         "additionalProperties": false,
        //         "required": [
        //             "chargingRateUnit",
        //             "chargingSchedulePeriod"
        //         ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, XML,  Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a GetCompositeSchedule response.
        /// </summary>
        /// <param name="Request">The GetCompositeSchedule request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static GetCompositeScheduleResponse Parse(GetCompositeScheduleRequest  Request,
                                                         XElement                     XML,
                                                         SourceRouting                Destination,
                                                         NetworkPath                  NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var getCompositeScheduleResponse,
                         out var errorResponse))
            {
                return getCompositeScheduleResponse;
            }

            throw new ArgumentException("The given XML representation of a GetCompositeSchedule response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetCompositeSchedule response.
        /// </summary>
        /// <param name="Request">The GetCompositeSchedule request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomGetCompositeScheduleResponseParser">An optional delegate to parse custom GetCompositeSchedule responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static GetCompositeScheduleResponse Parse(GetCompositeScheduleRequest                                 Request,
                                                         JObject                                                     JSON,
                                                         SourceRouting                                               Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTime?                                                   ResponseTimestamp                          = null,
                                                         CustomJObjectParserDelegate<GetCompositeScheduleResponse>?  CustomGetCompositeScheduleResponseParser   = null,
                                                         CustomJObjectParserDelegate<Signature>?                     CustomSignatureParser                      = null,
                                                         CustomJObjectParserDelegate<CustomData>?                    CustomCustomDataParser                     = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getCompositeScheduleResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetCompositeScheduleResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getCompositeScheduleResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetCompositeSchedule response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out GetCompositeScheduleResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a GetCompositeSchedule response.
        /// </summary>
        /// <param name="Request">The GetCompositeSchedule request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="GetCompositeScheduleResponse">The parsed GetCompositeSchedule response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(GetCompositeScheduleRequest                             Request,
                                       XElement                                                XML,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out GetCompositeScheduleResponse?  GetCompositeScheduleResponse,
                                       [NotNullWhen(false)] out String?                        ErrorResponse)
        {

            try
            {

                GetCompositeScheduleResponse = new GetCompositeScheduleResponse(

                                                   Request,

                                                   XML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                           GetCompositeScheduleStatusExtensions.Parse),

                                                   XML.MapValueOrNull     (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                           Connector_Id.Parse),

                                                   XML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "scheduleStart",
                                                                           DateTime.Parse),

                                                   XML.MapElement         (OCPPNS.OCPPv1_6_CP + "chargingSchedule",
                                                                           ChargingSchedule.Parse)

                                               );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetCompositeScheduleResponse  = null;
                ErrorResponse                 = "The given JSON representation of a GetCompositeSchedule response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out GetCompositeScheduleResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetCompositeSchedule response.
        /// </summary>
        /// <param name="Request">The GetCompositeSchedule request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="GetCompositeScheduleResponse">The parsed GetCompositeSchedule response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomGetCompositeScheduleResponseParser">An optional delegate to parse custom GetCompositeSchedule responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(GetCompositeScheduleRequest                                 Request,
                                       JObject                                                     JSON,
                                       SourceRouting                                               Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out GetCompositeScheduleResponse?      GetCompositeScheduleResponse,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTime?                                                   ResponseTimestamp                          = null,
                                       CustomJObjectParserDelegate<GetCompositeScheduleResponse>?  CustomGetCompositeScheduleResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                     CustomSignatureParser                      = null,
                                       CustomJObjectParserDelegate<CustomData>?                    CustomCustomDataParser                     = null)
        {

            try
            {

                GetCompositeScheduleResponse = null;

                #region GetCompositeScheduleStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "GetCompositeSchedule status",
                                       GetCompositeScheduleStatusExtensions.Parse,
                                       out GetCompositeScheduleStatus GetCompositeScheduleStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId                   [optional]

                if (JSON.ParseOptional("connectorId",
                                       "connector identification",
                                       Connector_Id.TryParse,
                                       out Connector_Id? ConnectorId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ScheduleStart                 [optional]

                if (JSON.ParseOptional("scheduleStart",
                                       "schedule start",
                                       out DateTime? ScheduleStart,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingSchedule              [optional]

                if (JSON.ParseOptionalJSON("chargingSchedule",
                                           "availability status",
                                           OCPPv1_6.ChargingSchedule.TryParse,
                                           out ChargingSchedule ChargingSchedule,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                    [optional, OCPP_CSE]

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

                #region CustomData                    [optional]

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


                GetCompositeScheduleResponse = new GetCompositeScheduleResponse(

                                                   Request,
                                                   GetCompositeScheduleStatus,
                                                   ConnectorId,
                                                   ScheduleStart,
                                                   ChargingSchedule,

                                                   null,
                                                   ResponseTimestamp,

                                                   Destination,
                                                   NetworkPath,

                                                   null,
                                                   null,
                                                   Signatures,

                                                   CustomData

                                               );

                if (CustomGetCompositeScheduleResponseParser is not null)
                    GetCompositeScheduleResponse = CustomGetCompositeScheduleResponseParser(JSON,
                                                                                            GetCompositeScheduleResponse);

                return true;

            }
            catch (Exception e)
            {
                GetCompositeScheduleResponse  = null;
                ErrorResponse                 = "The given JSON representation of a GetCompositeSchedule response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "getCompositeScheduleResponse",

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",               Status.AsText()),

                   ConnectorId != null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",    ConnectorId.ToString())
                       : null,

                   ScheduleStart.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "scheduleStart",  ScheduleStart.Value.ToIso8601())
                       : null,

                   ChargingSchedule?.ToXML()

               );

        #endregion

        #region ToJSON(CustomGetCompositeScheduleResponseSerializer = null, CustomChargingScheduleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCompositeScheduleResponseSerializer">A delegate to serialize custom GetCompositeSchedule responses.</param>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedule requests.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetCompositeScheduleResponse>?  CustomGetCompositeScheduleResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?              CustomChargingScheduleSerializer               = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?        CustomChargingSchedulePeriodSerializer         = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",             Status.             AsText()),

                           ConnectorId.HasValue
                               ? new JProperty("connectorId",        ConnectorId.  Value.Value)
                               : null,

                           ScheduleStart.HasValue
                               ? new JProperty("scheduleStart",      ScheduleStart.Value.ToIso8601())
                               : null,

                           ChargingSchedule is not null
                               ? new JProperty("chargingSchedule",   ChargingSchedule.   ToJSON(CustomChargingScheduleSerializer,
                                                                                                CustomChargingSchedulePeriodSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetCompositeScheduleResponseSerializer is not null
                       ? CustomGetCompositeScheduleResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetCompositeSchedule failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetCompositeSchedule request.</param>
        public static GetCompositeScheduleResponse RequestError(GetCompositeScheduleRequest  Request,
                                                                EventTracking_Id             EventTrackingId,
                                                                ResultCode                   ErrorCode,
                                                                String?                      ErrorDescription    = null,
                                                                JObject?                     ErrorDetails        = null,
                                                                DateTime?                    ResponseTimestamp   = null,

                                                                SourceRouting?               Destination         = null,
                                                                NetworkPath?                 NetworkPath         = null,

                                                                IEnumerable<KeyPair>?        SignKeys            = null,
                                                                IEnumerable<SignInfo>?       SignInfos           = null,
                                                                IEnumerable<Signature>?      Signatures          = null,

                                                                CustomData?                  CustomData          = null)

            => new (

                   Request,
                   GetCompositeScheduleStatus.Rejected,
                   null,
                   null,
                   null,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The GetCompositeSchedule failed.
        /// </summary>
        /// <param name="Request">The GetCompositeSchedule request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetCompositeScheduleResponse FormationViolation(GetCompositeScheduleRequest  Request,
                                                                      String                       ErrorDescription)

            => new (Request,
                    GetCompositeScheduleStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetCompositeSchedule failed.
        /// </summary>
        /// <param name="Request">The GetCompositeSchedule request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetCompositeScheduleResponse SignatureError(GetCompositeScheduleRequest  Request,
                                                                  String                       ErrorDescription)

            => new (Request,
                    GetCompositeScheduleStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetCompositeSchedule failed.
        /// </summary>
        /// <param name="Request">The GetCompositeSchedule request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetCompositeScheduleResponse Failed(GetCompositeScheduleRequest  Request,
                                                          String?                      Description   = null)

            => new (Request,
                    GetCompositeScheduleStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The GetCompositeSchedule failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetCompositeSchedule request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetCompositeScheduleResponse ExceptionOccured(GetCompositeScheduleRequest  Request,
                                                                    Exception                    Exception)

            => new (Request,
                    GetCompositeScheduleStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetCompositeScheduleResponse1, GetCompositeScheduleResponse2)

        /// <summary>
        /// Compares two GetCompositeSchedule responses for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleResponse1">A GetCompositeSchedule response.</param>
        /// <param name="GetCompositeScheduleResponse2">Another GetCompositeSchedule response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetCompositeScheduleResponse? GetCompositeScheduleResponse1,
                                           GetCompositeScheduleResponse? GetCompositeScheduleResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetCompositeScheduleResponse1, GetCompositeScheduleResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetCompositeScheduleResponse1 is null || GetCompositeScheduleResponse2 is null)
                return false;

            return GetCompositeScheduleResponse1.Equals(GetCompositeScheduleResponse2);

        }

        #endregion

        #region Operator != (GetCompositeScheduleResponse1, GetCompositeScheduleResponse2)

        /// <summary>
        /// Compares two GetCompositeSchedule responses for inequality.
        /// </summary>
        /// <param name="GetCompositeScheduleResponse1">A GetCompositeSchedule response.</param>
        /// <param name="GetCompositeScheduleResponse2">Another GetCompositeSchedule response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetCompositeScheduleResponse? GetCompositeScheduleResponse1,
                                           GetCompositeScheduleResponse? GetCompositeScheduleResponse2)

            => !(GetCompositeScheduleResponse1 == GetCompositeScheduleResponse2);

        #endregion

        #endregion

        #region IEquatable<GetCompositeScheduleResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetCompositeSchedule responses for equality.
        /// </summary>
        /// <param name="Object">A GetCompositeSchedule response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCompositeScheduleResponse getCompositeScheduleResponse &&
                   Equals(getCompositeScheduleResponse);

        #endregion

        #region Equals(GetCompositeScheduleResponse)

        /// <summary>
        /// Compares two GetCompositeSchedule responses for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleResponse">A GetCompositeSchedule response to compare with.</param>
        public override Boolean Equals(GetCompositeScheduleResponse? GetCompositeScheduleResponse)

            => GetCompositeScheduleResponse is not null &&

               Status.Equals(GetCompositeScheduleResponse.Status) &&

            ((!ConnectorId.  HasValue       && !GetCompositeScheduleResponse.ConnectorId.  HasValue)       ||
              (ConnectorId.  HasValue       &&  GetCompositeScheduleResponse.ConnectorId.  HasValue       && ConnectorId.        Equals(GetCompositeScheduleResponse.ConnectorId)))         &&

            ((!ScheduleStart.HasValue       && !GetCompositeScheduleResponse.ScheduleStart.HasValue)       ||
              (ScheduleStart.HasValue       &&  GetCompositeScheduleResponse.ScheduleStart.HasValue       && ScheduleStart.Value.Equals(GetCompositeScheduleResponse.ScheduleStart.Value))) &&

             ((ChargingSchedule is     null && GetCompositeScheduleResponse. ChargingSchedule is     null) ||
              (ChargingSchedule is not null && GetCompositeScheduleResponse. ChargingSchedule is not null && ChargingSchedule.   Equals(GetCompositeScheduleResponse.ChargingSchedule)));

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

                   Status,

                   ConnectorId.HasValue
                       ? $" at connector '{ConnectorId}'"
                       : "",

                   ScheduleStart.HasValue
                       ? $", start at {ScheduleStart.Value.ToIso8601()}"
                       : "",

                   ChargingSchedule is not null
                       ? ", has a charging schedule"
                       : ""

               );

        #endregion

    }

}
