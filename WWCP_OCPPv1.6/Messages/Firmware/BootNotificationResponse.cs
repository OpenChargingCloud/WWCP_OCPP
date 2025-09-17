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
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A boot notification response.
    /// </summary>
    public class BootNotificationResponse : AResponse<BootNotificationRequest,
                                                      BootNotificationResponse>,
                                            IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/bootNotificationResponse");

        /// <summary>
        /// The default heartbeat interval in seconds.
        /// </summary>
        public static TimeSpan DefaultInterval = TimeSpan.FromMinutes(5);

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The registration status.
        /// </summary>
        public RegistrationStatus  Status               { get; }

        /// <summary>
        /// The current time at the central system.
        /// Should be UTC!
        /// </summary>
        public DateTimeOffset      CurrentTime          { get; }

        /// <summary>
        /// When the registration status is 'accepted', the interval defines
        /// the heartbeat interval in seconds.
        /// In all other cases, the value of the interval field indicates
        /// the minimum wait time before sending a next BootNotification
        /// request.
        /// </summary>
        public TimeSpan            HeartbeatInterval    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="CurrentTime">The current time at the central system. Should be UTC!</param>
        /// <param name="HeartbeatInterval">When the registration status is 'accepted', the interval defines the heartbeat interval in seconds. In all other cases, the value of the interval field indicates the minimum wait time before sending a next BootNotification request.</param>
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
        public BootNotificationResponse(BootNotificationRequest  Request,
                                        RegistrationStatus       Status,
                                        DateTimeOffset           CurrentTime,
                                        TimeSpan                 HeartbeatInterval,

                                        Result?                  Result                = null,
                                        DateTimeOffset?          ResponseTimestamp     = null,

                                        SourceRouting?           Destination           = null,
                                        NetworkPath?             NetworkPath           = null,

                                        IEnumerable<KeyPair>?    SignKeys              = null,
                                        IEnumerable<SignInfo>?   SignInfos             = null,
                                        IEnumerable<Signature>?  Signatures            = null,

                                        CustomData?              CustomData            = null,

                                        SerializationFormats?    SerializationFormat   = null,
                                        CancellationToken        CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat,
                   CancellationToken)

        {

            this.Status             = Status;
            this.CurrentTime        = CurrentTime;
            this.HeartbeatInterval  = HeartbeatInterval;

            unchecked
            {

                hashCode = this.Status.           GetHashCode() * 7 ^
                           this.CurrentTime.      GetHashCode() * 5 ^
                           this.HeartbeatInterval.GetHashCode() * 3 ^
                           base.                  GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:bootNotificationResponse>
        //
        //          <ns:status>?</ns:status>
        //          <ns:currentTime>?</ns:currentTime>
        //          <ns:interval>?</ns:interval>
        //
        //       </ns:bootNotificationResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:BootNotificationResponse",
        //     "title":    "BootNotificationResponse",
        //     "type":     "object",
        //     "properties": {
        //         "status": {
        //             "type":                 "string",
        //             "additionalProperties":  false,
        //             "enum": [
        //                 "Accepted",
        //                 "Pending",
        //                 "Rejected"
        //             ]
        //         },
        //         "currentTime": {
        //             "type":   "string",
        //             "format": "date-time"
        //         },
        //         "interval": {
        //             "type":   "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status",
        //         "currentTime",
        //         "interval"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, XML,  Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static BootNotificationResponse Parse(BootNotificationRequest  Request,
                                                     XElement                 XML,
                                                     SourceRouting            Destination,
                                                     NetworkPath              NetworkPath)
        {


            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var bootNotificationResponse,
                         out var errorResponse))
            {
                return bootNotificationResponse;
            }

            throw new ArgumentException("The given XML representation of a boot notification response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The BootNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomBootNotificationResponseParser">An optional delegate to parse custom BootNotification responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static BootNotificationResponse Parse(BootNotificationRequest                                 Request,
                                                     JObject                                                 JSON,
                                                     SourceRouting                                           Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTimeOffset?                                         ResponseTimestamp                      = null,
                                                     CustomJObjectParserDelegate<BootNotificationResponse>?  CustomBootNotificationResponseParser   = null,
                                                     CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                                     CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var bootNotificationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomBootNotificationResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return bootNotificationResponse;
            }

            throw new ArgumentException("The given JSON representation of a BootNotification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out BootNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="BootNotificationResponse">The parsed boot notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(BootNotificationRequest                             Request,
                                       XElement                                            XML,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out BootNotificationResponse?  BootNotificationResponse,
                                       [NotNullWhen(false)] out String?                    ErrorResponse)
        {

            try
            {

                BootNotificationResponse = new BootNotificationResponse(

                                               Request,

                                               XML.MapValueOrFail      (OCPPNS.OCPPv1_6_CS + "status",
                                                                        RegistrationStatus.Parse),

                                               XML.ParseTimestampOrFail(OCPPNS.OCPPv1_6_CS + "currentTime"),

                                               XML.ParseTimeSpanOrFail (OCPPNS.OCPPv1_6_CS + "interval")

                                           );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                BootNotificationResponse  = null;
                ErrorResponse             = "The given XML representation of a boot notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out BootNotificationResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The BootNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="BootNotificationResponse">The parsed BootNotification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomBootNotificationResponseParser">An optional delegate to parse custom BootNotification responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(BootNotificationRequest                                 Request,
                                       JObject                                                 JSON,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out BootNotificationResponse?      BootNotificationResponse,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTimeOffset?                                         ResponseTimestamp                      = null,
                                       CustomJObjectParserDelegate<BootNotificationResponse>?  CustomBootNotificationResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                       CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            try
            {

                BootNotificationResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "registration status",
                                         OCPPv1_6.RegistrationStatus.TryParse,
                                         out RegistrationStatus RegistrationStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CurrentTime    [mandatory]

                if (!JSON.ParseMandatory("currentTime",
                                         "current time",
                                         out DateTime CurrentTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Interval       [mandatory]

                if (!JSON.ParseMandatory("interval",
                                         "heartbeat interval",
                                         out TimeSpan Interval,
                                         out ErrorResponse))
                {
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


                BootNotificationResponse = new BootNotificationResponse(

                                               Request,
                                               RegistrationStatus,
                                               CurrentTime,
                                               Interval,

                                               null,
                                               ResponseTimestamp,

                                               Destination,
                                               NetworkPath,

                                               null,
                                               null,
                                               Signatures,

                                               CustomData

                                           );

                if (CustomBootNotificationResponseParser is not null)
                    BootNotificationResponse = CustomBootNotificationResponseParser(JSON,
                                                                                    BootNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                BootNotificationResponse  = null;
                ErrorResponse             = "The given JSON representation of a boot notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "bootNotificationResponse",

                   new XElement(OCPPNS.OCPPv1_6_CS + "status",       Status.           ToString()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "currentTime",  CurrentTime.      ToISO8601()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "interval",     (UInt32) HeartbeatInterval.TotalSeconds)

               );

        #endregion

        #region ToJSON(CustomBootNotificationResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBootNotificationResponseSerializer">A delegate to serialize custom boot notification responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BootNotificationResponse>?  CustomBootNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",        Status.     ToString()),
                                 new JProperty("currentTime",   CurrentTime.ToISO8601()),
                                 new JProperty("interval",      (UInt32) HeartbeatInterval.TotalSeconds),

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomBootNotificationResponseSerializer is not null
                       ? CustomBootNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The BootNotification failed because of a request error.
        /// </summary>
        /// <param name="Request">The BootNotification request leading to this response.</param>
        public static BootNotificationResponse RequestError(BootNotificationRequest  Request,
                                                            EventTracking_Id         EventTrackingId,
                                                            ResultCode               ErrorCode,
                                                            String?                  ErrorDescription    = null,
                                                            JObject?                 ErrorDetails        = null,
                                                            DateTimeOffset?          ResponseTimestamp   = null,

                                                            SourceRouting?           Destination         = null,
                                                            NetworkPath?             NetworkPath         = null,

                                                            IEnumerable<KeyPair>?    SignKeys            = null,
                                                            IEnumerable<SignInfo>?   SignInfos           = null,
                                                            IEnumerable<Signature>?  Signatures          = null,

                                                            CustomData?              CustomData          = null)

            => new (

                   Request,
                   RegistrationStatus.Rejected,
                   Timestamp.Now,
                   DefaultInterval,
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
        /// The BootNotification failed.
        /// </summary>
        /// <param name="Request">The BootNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static BootNotificationResponse FormationViolation(BootNotificationRequest  Request,
                                                                  String                   ErrorDescription)

            => new (Request,
                    RegistrationStatus.Rejected,
                    Timestamp.Now,
                    DefaultInterval,
                    Result:               Result.FormationViolation(
                                              $"Invalid data format: {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The BootNotification failed.
        /// </summary>
        /// <param name="Request">The BootNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static BootNotificationResponse SignatureError(BootNotificationRequest  Request,
                                                              String                   ErrorDescription)

            => new (Request,
                    RegistrationStatus.SignatureError,
                    Timestamp.Now,
                    DefaultInterval,
                    Result:               Result.SignatureError(
                                              $"Invalid signature(s): {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The BootNotification failed.
        /// </summary>
        /// <param name="Request">The BootNotification request.</param>
        /// <param name="Description">An optional error description.</param>
        public static BootNotificationResponse Failed(BootNotificationRequest  Request,
                                                      String?                  Description   = null)

            => new (Request,
                    RegistrationStatus.Error,
                    Timestamp.Now,
                    DefaultInterval,
                    Result:               Result.Server(Description),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The BootNotification failed because of an exception.
        /// </summary>
        /// <param name="Request">The BootNotification request.</param>
        /// <param name="Exception">The exception.</param>
        public static BootNotificationResponse ExceptionOccurred(BootNotificationRequest  Request,
                                                                Exception                Exception)

            => new (Request,
                    RegistrationStatus.Error,
                    Timestamp.Now,
                    DefaultInterval,
                    Result:               Result.FromException(Exception),
                    SerializationFormat:  Request.SerializationFormat);

        #endregion


        #region Operator overloading

        #region Operator == (BootNotificationResponse1, BootNotificationResponse2)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="BootNotificationResponse1">A boot notification response.</param>
        /// <param name="BootNotificationResponse2">Another boot notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (BootNotificationResponse? BootNotificationResponse1,
                                           BootNotificationResponse? BootNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(BootNotificationResponse1, BootNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (BootNotificationResponse1 is null || BootNotificationResponse2 is null)
                return false;

            return BootNotificationResponse1.Equals(BootNotificationResponse2);

        }

        #endregion

        #region Operator != (BootNotificationResponse1, BootNotificationResponse2)

        /// <summary>
        /// Compares two boot notification responses for inequality.
        /// </summary>
        /// <param name="BootNotificationResponse1">A boot notification response.</param>
        /// <param name="BootNotificationResponse2">Another boot notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (BootNotificationResponse? BootNotificationResponse1,
                                           BootNotificationResponse? BootNotificationResponse2)

            => !(BootNotificationResponse1 == BootNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<BootNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="Object">A boot notification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BootNotificationResponse bootNotificationResponse &&
                   Equals(bootNotificationResponse);

        #endregion

        #region Equals(BootNotificationResponse)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="BootNotificationResponse">A boot notification response to compare with.</param>
        public override Boolean Equals(BootNotificationResponse? BootNotificationResponse)

            => BootNotificationResponse is not null                           &&

               Status.           Equals(BootNotificationResponse.Status)      &&
               CurrentTime.      Equals(BootNotificationResponse.CurrentTime) &&
               HeartbeatInterval.Equals(BootNotificationResponse.HeartbeatInterval);

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

            => $"{Status} ({CurrentTime.ToISO8601()}, {HeartbeatInterval.TotalSeconds} sec(s))";

        #endregion

    }

}
