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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The UpdateFirmware request.
    /// </summary>
    public class UpdateFirmwareRequest : ARequest<UpdateFirmwareRequest>,
                                         IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/updateFirmwareRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The URL where to download the firmware.
        /// </summary>
        public URL            FirmwareURL          { get; }

        /// <summary>
        /// The timestamp when the charge point shall retrieve the firmware.
        /// </summary>
        public DateTime       RetrieveTimestamp    { get; }

        /// <summary>
        /// The optional number of retries of a charge point for trying to
        /// download the firmware before giving up. If this field is not
        /// present, it is left to the charge point to decide how many times
        /// it wants to retry.
        /// </summary>
        public Byte?          Retries              { get; }

        /// <summary>
        /// The interval after which a retry may be attempted. If this field
        /// is not present, it is left to charge point to decide how long to
        /// wait between attempts.
        /// </summary>
        public TimeSpan?      RetryInterval        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new UpdateFirmware request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="FirmwareURL">The URL where to download the firmware.</param>
        /// <param name="RetrieveTimestamp">The timestamp when the charge point shall retrieve the firmware.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
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
        public UpdateFirmwareRequest(SourceRouting            Destination,
                                     URL                      FirmwareURL,
                                     DateTime                 RetrieveTimestamp,
                                     Byte?                    Retries               = null,
                                     TimeSpan?                RetryInterval         = null,

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
                   nameof(UpdateFirmwareRequest)[..^7],

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

            this.FirmwareURL        = FirmwareURL;
            this.RetrieveTimestamp  = RetrieveTimestamp;
            this.Retries            = Retries;
            this.RetryInterval      = RetryInterval;

            unchecked
            {

                hashCode = this.FirmwareURL.      GetHashCode()       * 11 ^
                           this.RetrieveTimestamp.GetHashCode()       *  7 ^
                          (this.Retries?.         GetHashCode() ?? 0) *  5 ^
                          (this.RetryInterval?.   GetHashCode() ?? 0) *  3 ^
                           base.                  GetHashCode();

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
        //       <ns:updateFirmwareRequest>
        //
        //          <ns:retrieveDate>?</ns:retrieveDate>
        //          <ns:location>?</ns:location>
        //
        //          <!--Optional:-->
        //          <ns:retries>?</ns:retries>
        //
        //          <!--Optional:-->
        //          <ns:retryInterval>?</ns:retryInterval>
        //
        //       </ns:updateFirmwareRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:UpdateFirmwareRequest",
        //     "title":   "UpdateFirmwareRequest",
        //     "type":    "object",
        //     "properties": {
        //         "location": {
        //             "type": "string",
        //             "format": "uri"
        //         },
        //         "retries": {
        //             "type": "integer"
        //         },
        //         "retrieveDate": {
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "retryInterval": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "location",
        //         "retrieveDate"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of an UpdateFirmware request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        public static UpdateFirmwareRequest Parse(XElement       XML,
                                                  Request_Id     RequestId,
                                                  SourceRouting  Destination,
                                                  NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var updateFirmwareRequest,
                         out var errorResponse))
            {
                return updateFirmwareRequest;
            }

            throw new ArgumentException("The given XML representation of an UpdateFirmware request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an UpdateFirmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomUpdateFirmwareRequestParser">An optional delegate to parse custom UpdateFirmware requests.</param>
        public static UpdateFirmwareRequest Parse(JObject                                              JSON,
                                                  Request_Id                                           RequestId,
                                                  SourceRouting                                        Destination,
                                                  NetworkPath                                          NetworkPath,
                                                  DateTime?                                            RequestTimestamp                    = null,
                                                  TimeSpan?                                            RequestTimeout                      = null,
                                                  EventTracking_Id?                                    EventTrackingId                     = null,
                                                  CustomJObjectParserDelegate<UpdateFirmwareRequest>?  CustomUpdateFirmwareRequestParser   = null,
                                                  CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                                  CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var updateFirmwareRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomUpdateFirmwareRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return updateFirmwareRequest;
            }

            throw new ArgumentException("The given JSON representation of an UpdateFirmware request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out UpdateFirmwareRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of an UpdateFirmware request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="UpdateFirmwareRequest">The parsed UpdateFirmware request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                         XML,
                                       Request_Id                                       RequestId,
                                       SourceRouting                                    Destination,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out UpdateFirmwareRequest?  UpdateFirmwareRequest,
                                       [NotNullWhen(false)] out String?                 ErrorResponse)
        {

            try
            {

                UpdateFirmwareRequest = new UpdateFirmwareRequest(

                                            Destination,

                                            URL.Parse(XML.ElementValueOrFail(OCPPNS.OCPPv1_6_CP + "location")),

                                            XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "retrieveDate",
                                                                                        DateTime.Parse),

                                            XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "retries",
                                                                                        Byte.Parse),

                                            XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "retryInterval",
                                                                                        s => TimeSpan.FromSeconds(UInt32.Parse(s))),

                                            RequestId: RequestId,
                                            NetworkPath: NetworkPath

                                        );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UpdateFirmwareRequest  = null;
                ErrorResponse          = "The given XML representation of an UpdateFirmware request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out UpdateFirmwareRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an UpdateFirmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="UpdateFirmwareRequest">The parsed UpdateFirmware request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomUpdateFirmwareRequestParser">A delegate to parse custom UpdateFirmware requests.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       Request_Id                                           RequestId,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out UpdateFirmwareRequest?      UpdateFirmwareRequest,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       DateTime?                                            RequestTimestamp                    = null,
                                       TimeSpan?                                            RequestTimeout                      = null,
                                       EventTracking_Id?                                    EventTrackingId                     = null,
                                       CustomJObjectParserDelegate<UpdateFirmwareRequest>?  CustomUpdateFirmwareRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                       CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            try
            {

                UpdateFirmwareRequest = null;

                #region FirmwareURL          [mandatory]

                if (!JSON.ParseMandatory("location",
                                         "location",
                                         URL.TryParse,
                                         out URL FirmwareURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region RetrieveTimestamp    [mandatory]

                if (!JSON.ParseMandatory("retrieveDate",
                                         "retrieve date",
                                         out DateTime RetrieveDate,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Retries              [optional]

                if (JSON.ParseOptional("retries",
                                       "retries",
                                       out Byte? Retries,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RetryInterval        [optional]

                if (JSON.ParseOptional("retryInterval",
                                       "retry interval",
                                       out TimeSpan? RetryInterval,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                UpdateFirmwareRequest = new UpdateFirmwareRequest(

                                            Destination,
                                            FirmwareURL,
                                            RetrieveDate,
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

                if (CustomUpdateFirmwareRequestParser is not null)
                    UpdateFirmwareRequest = CustomUpdateFirmwareRequestParser(JSON,
                                                                              UpdateFirmwareRequest);

                return true;

            }
            catch (Exception e)
            {
                UpdateFirmwareRequest  = null;
                ErrorResponse          = "The given JSON representation of an UpdateFirmware request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new(OCPPNS.OCPPv1_6_CP + "getDiagnosticsRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "retrieveDate", RetrieveTimestamp.ToISO8601()),
                   new XElement(OCPPNS.OCPPv1_6_CP + "location",     FirmwareURL.      ToString()),

                   Retries.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "retries", Retries.Value)
                       : null,

                   RetryInterval.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "retryInterval", (UInt64)RetryInterval.Value.TotalSeconds)
                       : null

               );

        #endregion

        #region ToJSON(CustomUpdateFirmwareRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateFirmwareRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateFirmwareRequest>?  CustomUpdateFirmwareRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?              CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("retrieveDate",    RetrieveTimestamp.ToISO8601()),
                                 new JProperty("location",        FirmwareURL.      ToString()),

                           Retries.HasValue
                               ? new JProperty("retries",         Retries.Value)
                               : null,

                           RetryInterval.HasValue
                               ? new JProperty("retryInterval",   (UInt64) RetryInterval.Value.TotalSeconds)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.       ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUpdateFirmwareRequestSerializer is not null
                       ? CustomUpdateFirmwareRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UpdateFirmwareRequest1, UpdateFirmwareRequest2)

        /// <summary>
        /// Compares two UpdateFirmware requests for equality.
        /// </summary>
        /// <param name="UpdateFirmwareRequest1">An UpdateFirmware request.</param>
        /// <param name="UpdateFirmwareRequest2">Another UpdateFirmware request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator ==(UpdateFirmwareRequest? UpdateFirmwareRequest1,
                                           UpdateFirmwareRequest? UpdateFirmwareRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateFirmwareRequest1, UpdateFirmwareRequest2))
                return true;

            // If one is null, but not both, return false.
            if (UpdateFirmwareRequest1 is null || UpdateFirmwareRequest2 is null)
                return false;

            return UpdateFirmwareRequest1.Equals(UpdateFirmwareRequest2);

        }

        #endregion

        #region Operator != (UpdateFirmwareRequest1, UpdateFirmwareRequest2)

        /// <summary>
        /// Compares two UpdateFirmware requests for inequality.
        /// </summary>
        /// <param name="UpdateFirmwareRequest1">An UpdateFirmware request.</param>
        /// <param name="UpdateFirmwareRequest2">Another UpdateFirmware request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator !=(UpdateFirmwareRequest? UpdateFirmwareRequest1,
                                           UpdateFirmwareRequest? UpdateFirmwareRequest2)

            => !(UpdateFirmwareRequest1 == UpdateFirmwareRequest2);

        #endregion

        #endregion

        #region IEquatable<UpdateFirmwareRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UpdateFirmware requests for equality.
        /// </summary>
        /// <param name="Object">An UpdateFirmware request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateFirmwareRequest updateFirmwareRequest &&
                   Equals(updateFirmwareRequest);

        #endregion

        #region Equals(UpdateFirmwareRequest)

        /// <summary>
        /// Compares two UpdateFirmware requests for equality.
        /// </summary>
        /// <param name="UpdateFirmwareRequest">An UpdateFirmware request to compare with.</param>
        public override Boolean Equals(UpdateFirmwareRequest? UpdateFirmwareRequest)

            => UpdateFirmwareRequest is not null &&

               FirmwareURL.Equals(UpdateFirmwareRequest.FirmwareURL) &&
               RetrieveTimestamp.Equals(UpdateFirmwareRequest.RetrieveTimestamp) &&

               ((!Retries.HasValue && !UpdateFirmwareRequest.Retries.HasValue) ||
                 (Retries.HasValue && UpdateFirmwareRequest.Retries.HasValue && Retries.Value.Equals(UpdateFirmwareRequest.Retries.Value))) &&

               ((!RetryInterval.HasValue && !UpdateFirmwareRequest.RetryInterval.HasValue) ||
                 (RetryInterval.HasValue && UpdateFirmwareRequest.RetryInterval.HasValue && RetryInterval.Value.Equals(UpdateFirmwareRequest.RetryInterval.Value))) &&

               base.GenericEquals(UpdateFirmwareRequest);

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

                   $"'{FirmwareURL.ToString().SubstringMax(20)}' till '{RetrieveTimestamp}'",

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
