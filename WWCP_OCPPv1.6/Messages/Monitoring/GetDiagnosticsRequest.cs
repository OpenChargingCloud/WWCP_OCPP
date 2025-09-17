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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The GetDiagnostics request.
    /// </summary>
    public class GetDiagnosticsRequest : ARequest<GetDiagnosticsRequest>,
                                         IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/getDiagnosticsRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The URI where the diagnostics file shall be uploaded to.
        /// </summary>
        public String           Location         { get; }

        /// <summary>
        /// The timestamp of the oldest logging information to include in
        /// the diagnostics.
        /// </summary>
        public DateTimeOffset?  StartTime        { get; }

        /// <summary>
        /// The timestamp of the latest logging information to include in
        /// the diagnostics.
        /// </summary>
        public DateTimeOffset?  StopTime         { get; }

        /// <summary>
        /// The optional number of retries of a charge point for trying to
        /// upload the diagnostics before giving up. If this field is not
        /// present, it is left to the charge point to decide how many times
        /// it wants to retry.
        /// </summary>
        public Byte?            Retries          { get; }

        /// <summary>
        /// The interval after which a retry may be attempted. If this field
        /// is not present, it is left to charge point to decide how long to
        /// wait between attempts.
        /// </summary>
        public TimeSpan?        RetryInterval    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetDiagnostics request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Location">The URI where the diagnostics file shall be uploaded to.</param>
        /// <param name="StartTime">The timestamp of the oldest logging information to include in the diagnostics.</param>
        /// <param name="StopTime">The timestamp of the latest logging information to include in the diagnostics.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to upload the diagnostics before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
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
        public GetDiagnosticsRequest(SourceRouting            Destination,
                                     String                   Location,
                                     DateTimeOffset?          StartTime             = null,
                                     DateTimeOffset?          StopTime              = null,
                                     Byte?                    Retries               = null,
                                     TimeSpan?                RetryInterval         = null,

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
                   nameof(GetDiagnosticsRequest)[..^7],

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

            #region Initial checks

            Location = Location.Trim();

            if (Location.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Location), "The given location must not be null or empty!");

            #endregion

            this.Location       = Location;
            this.StartTime      = StartTime;
            this.StopTime       = StopTime;
            this.Retries        = Retries;
            this.RetryInterval  = RetryInterval;


            unchecked
            {

                hashCode = this.Location.      GetHashCode()       * 13 ^
                          (this.StartTime?.    GetHashCode() ?? 0) * 11 ^
                          (this.StopTime?.     GetHashCode() ?? 0) *  7 ^
                          (this.Retries?.      GetHashCode() ?? 0) *  5 ^
                          (this.RetryInterval?.GetHashCode() ?? 0) *  3 ^
                           base.               GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //
        //    <soap:Header>
        //       ...
        //    </soap:Header>
        //
        //    <soap:Body>
        //       <ns:getDiagnosticsRequest>
        //
        //          <ns:location>?</ns:location>
        //
        //          <!--Optional:-->
        //          <ns:startTime>?</ns:startTime>
        //
        //          <!--Optional:-->
        //          <ns:stopTime>?</ns:stopTime>
        //
        //          <!--Optional:-->
        //          <ns:retries>?</ns:retries>
        //
        //          <!--Optional:-->
        //          <ns:retryInterval>?</ns:retryInterval>
        //
        //       </ns:getDiagnosticsRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetDiagnosticsRequest",
        //     "title":   "GetDiagnosticsRequest",
        //     "type":    "object",
        //     "properties": {
        //         "location": {
        //             "type": "string",
        //             "format": "uri"
        //         },
        //         "retries": {
        //             "type": "integer"
        //         },
        //         "retryInterval": {
        //             "type": "integer"
        //         },
        //         "startTime": {
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "stopTime": {
        //             "type": "string",
        //             "format": "date-time"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "location"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a GetDiagnostics request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static GetDiagnosticsRequest Parse(XElement       XML,
                                                  Request_Id     RequestId,
                                                  SourceRouting  Destination,
                                                  NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getDiagnosticsRequest,
                         out var errorResponse))
            {
                return getDiagnosticsRequest;
            }

            throw new ArgumentException("The given XML representation of a GetDiagnostics request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetDiagnostics request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetDiagnosticsRequestParser">A delegate to parse custom GetDiagnostics requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static GetDiagnosticsRequest Parse(JObject                                              JSON,
                                                  Request_Id                                           RequestId,
                                                  SourceRouting                                        Destination,
                                                  NetworkPath                                          NetworkPath,
                                                  DateTimeOffset?                                      RequestTimestamp                    = null,
                                                  TimeSpan?                                            RequestTimeout                      = null,
                                                  EventTracking_Id?                                    EventTrackingId                     = null,
                                                  CustomJObjectParserDelegate<GetDiagnosticsRequest>?  CustomGetDiagnosticsRequestParser   = null,
                                                  CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                                  CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getDiagnosticsRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetDiagnosticsRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getDiagnosticsRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetDiagnostics request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out GetDiagnosticsRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a GetDiagnostics request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetDiagnosticsRequest">The parsed GetDiagnostics request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                         XML,
                                       Request_Id                                       RequestId,
                                       SourceRouting                                    Destination,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out GetDiagnosticsRequest?  GetDiagnosticsRequest,
                                       [NotNullWhen(false)] out String?                 ErrorResponse)
        {

            try
            {

                GetDiagnosticsRequest = new GetDiagnosticsRequest(

                                            Destination,

                                            XML.ElementValueOrFail(OCPPNS.OCPPv1_6_CP + "location"),

                                            XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "startTime",
                                                                   DateTime.Parse),

                                            XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "stopTime",
                                                                   DateTime.Parse),

                                            XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "retries",
                                                                   Byte.Parse),

                                            XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "retryInterval",
                                                                   s => TimeSpan.FromSeconds(UInt32.Parse(s))),

                                            RequestId:    RequestId,
                                            NetworkPath:  NetworkPath

                                        );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetDiagnosticsRequest  = null;
                ErrorResponse          = "The given XML representation of a GetDiagnostics request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out GetDiagnosticsRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetDiagnostics request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetDiagnosticsRequest">The parsed GetDiagnostics request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetDiagnosticsRequestParser">A delegate to parse custom GetDiagnostics requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       Request_Id                                           RequestId,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out GetDiagnosticsRequest?      GetDiagnosticsRequest,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       DateTimeOffset?                                      RequestTimestamp                    = null,
                                       TimeSpan?                                            RequestTimeout                      = null,
                                       EventTracking_Id?                                    EventTrackingId                     = null,
                                       CustomJObjectParserDelegate<GetDiagnosticsRequest>?  CustomGetDiagnosticsRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                       CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            try
            {

                GetDiagnosticsRequest = null;

                #region Location         [mandatory]

                if (!JSON.ParseMandatoryText("location",
                                             "location",
                                             out String Location,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StartTime        [optional]

                if (JSON.ParseOptional("startTime",
                                       "start time",
                                       out DateTime? StartTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StopTime         [optional]

                if (JSON.ParseOptional("stopTime",
                                       "stop time",
                                       out DateTime? StopTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Retries          [optional]

                if (JSON.ParseOptional("retries",
                                       "retries",
                                       out Byte? Retries,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RetryInterval    [optional]

                if (JSON.ParseOptional("retryInterval",
                                       "retry interval",
                                       out TimeSpan? RetryInterval,
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
                                              out HashSet<Signature> Signatures,
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
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetDiagnosticsRequest = new GetDiagnosticsRequest(

                                            Destination,
                                            Location,
                                            StartTime,
                                            StopTime,
                                            Retries,
                                            RetryInterval,

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

                if (CustomGetDiagnosticsRequestParser is not null)
                    GetDiagnosticsRequest = CustomGetDiagnosticsRequestParser(JSON,
                                                                              GetDiagnosticsRequest);

                return true;

            }
            catch (Exception e)
            {
                GetDiagnosticsRequest  = null;
                ErrorResponse          = "The given JSON representation of a GetDiagnostics request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "getDiagnosticsRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "location",             Location),

                   StartTime.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "startTime",      StartTime.Value.ToISO8601())
                       : null,

                   StopTime.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "stopTime",       StopTime.Value.ToISO8601())
                       : null,

                   Retries.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "retries",        Retries.Value)
                       : null,

                   RetryInterval.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "retryInterval",  RetryInterval.Value.TotalSeconds)
                       : null

               );

        #endregion

        #region ToJSON(CustomGetDiagnosticsRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetDiagnosticsRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetDiagnosticsRequest>?  CustomGetDiagnosticsRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?              CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                           new JProperty("location",             Location),

                           StartTime.HasValue
                               ? new JProperty("startTime",      StartTime.Value.ToISO8601())
                               : null,

                           StopTime.HasValue
                               ? new JProperty("stopTime",       StopTime. Value.ToISO8601())
                               : null,

                           Retries.HasValue
                               ? new JProperty("retries",        Retries.  Value.ToString())
                               : null,

                           RetryInterval.HasValue
                               ? new JProperty("retryInterval",  (UInt64) RetryInterval.Value.TotalSeconds)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",     new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                            CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetDiagnosticsRequestSerializer is not null
                       ? CustomGetDiagnosticsRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetDiagnosticsRequest1, GetDiagnosticsRequest2)

        /// <summary>
        /// Compares two GetDiagnostics requests for equality.
        /// </summary>
        /// <param name="GetDiagnosticsRequest1">A GetDiagnostics request.</param>
        /// <param name="GetDiagnosticsRequest2">Another GetDiagnostics request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetDiagnosticsRequest? GetDiagnosticsRequest1,
                                           GetDiagnosticsRequest? GetDiagnosticsRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetDiagnosticsRequest1, GetDiagnosticsRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetDiagnosticsRequest1 is null || GetDiagnosticsRequest2 is null)
                return false;

            return GetDiagnosticsRequest1.Equals(GetDiagnosticsRequest2);

        }

        #endregion

        #region Operator != (GetDiagnosticsRequest1, GetDiagnosticsRequest2)

        /// <summary>
        /// Compares two GetDiagnostics requests for inequality.
        /// </summary>
        /// <param name="GetDiagnosticsRequest1">A GetDiagnostics request.</param>
        /// <param name="GetDiagnosticsRequest2">Another GetDiagnostics request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetDiagnosticsRequest? GetDiagnosticsRequest1,
                                           GetDiagnosticsRequest? GetDiagnosticsRequest2)

            => !(GetDiagnosticsRequest1 == GetDiagnosticsRequest2);

        #endregion

        #endregion

        #region IEquatable<GetDiagnosticsRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetDiagnostics requests for equality.
        /// </summary>
        /// <param name="Object">A GetDiagnostics request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetDiagnosticsRequest getDiagnosticsRequest &&
                   Equals(getDiagnosticsRequest);

        #endregion

        #region Equals(GetDiagnosticsRequest)

        /// <summary>
        /// Compares two GetDiagnostics requests for equality.
        /// </summary>
        /// <param name="GetDiagnosticsRequest">A GetDiagnostics request to compare with.</param>
        public override Boolean Equals(GetDiagnosticsRequest? GetDiagnosticsRequest)

            => GetDiagnosticsRequest is not null &&

               Location.Equals(GetDiagnosticsRequest.Location) &&

            ((!StartTime.    HasValue && !GetDiagnosticsRequest.StartTime.    HasValue) ||
              (StartTime.    HasValue &&  GetDiagnosticsRequest.StartTime.    HasValue && StartTime.    Value.Equals(GetDiagnosticsRequest.StartTime.    Value))) &&

            ((!StopTime.     HasValue && !GetDiagnosticsRequest.StopTime.     HasValue) ||
              (StopTime.     HasValue &&  GetDiagnosticsRequest.StopTime.     HasValue && StopTime.     Value.Equals(GetDiagnosticsRequest.StopTime.     Value))) &&

            ((!Retries.      HasValue && !GetDiagnosticsRequest.Retries.      HasValue) ||
              (Retries.      HasValue &&  GetDiagnosticsRequest.Retries.      HasValue && Retries.      Value.Equals(GetDiagnosticsRequest.Retries.      Value))) &&

            ((!RetryInterval.HasValue && !GetDiagnosticsRequest.RetryInterval.HasValue) ||
              (RetryInterval.HasValue &&  GetDiagnosticsRequest.RetryInterval.HasValue && RetryInterval.Value.Equals(GetDiagnosticsRequest.RetryInterval.Value))) &&

               base.GenericEquals(GetDiagnosticsRequest);

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

                   Location,

                   StartTime.HasValue
                       ? $", from {StartTime.Value.ToISO8601()}"
                       : "",

                   StopTime.HasValue
                       ? $", to {StopTime. Value.ToISO8601()}"
                       : "",

                   Retries.HasValue
                       ? $", {Retries.Value} retries"
                       : "",

                   RetryInterval.HasValue
                       ? $", retry interval {RetryInterval.Value.TotalSeconds} sec(s)"
                       : ""

               );

        #endregion

    }

}
