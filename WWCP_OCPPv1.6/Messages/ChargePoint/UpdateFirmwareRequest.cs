/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using System;
using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The UpdateFirmware request.
    /// </summary>
    public class UpdateFirmwareRequest : ARequest<UpdateFirmwareRequest>
    {

        #region Properties

        /// <summary>
        /// The URI where to download the firmware.
        /// </summary>
        public String     Location         { get; }

        /// <summary>
        /// The timestamp after which the charge point must retrieve the
        /// firmware.
        /// </summary>
        public DateTime   RetrieveDate     { get; }

        /// <summary>
        /// The optional number of retries of a charge point for trying to
        /// download the firmware before giving up. If this field is not
        /// present, it is left to the charge point to decide how many times
        /// it wants to retry.
        /// </summary>
        public Byte?      Retries          { get; }

        /// <summary>
        /// The interval after which a retry may be attempted. If this field
        /// is not present, it is left to charge point to decide how long to
        /// wait between attempts.
        /// </summary>
        public TimeSpan?  RetryInterval    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new UpdateFirmware request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Location">The URI where to download the firmware.</param>
        /// <param name="RetrieveDate">The timestamp after which the charge point must retrieve the firmware.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public UpdateFirmwareRequest(ChargeBox_Id  ChargeBoxId,
                                     String        Location,
                                     DateTime      RetrieveDate,
                                     Byte?         Retries            = null,
                                     TimeSpan?     RetryInterval      = null,

                                     Request_Id?   RequestId          = null,
                                     DateTime?     RequestTimestamp   = null)

            : base(ChargeBoxId,
                   "UpdateFirmware",
                   RequestId,
                   RequestTimestamp)

        {

            #region Initial checks

            Location = Location?.Trim();

            if (Location.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Location),  "The given location must not be null or empty!");

            #endregion

            this.Location       = Location;
            this.RetrieveDate   = RetrieveDate;
            this.Retries        = Retries;
            this.RetryInterval  = RetryInterval;

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

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of a UpdateFirmware request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UpdateFirmwareRequest Parse(XElement             XML,
                                                  Request_Id           RequestId,
                                                  ChargeBox_Id         ChargeBoxId,
                                                  OnExceptionDelegate  OnException = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out UpdateFirmwareRequest updateFirmwareRequest,
                         OnException))
            {
                return updateFirmwareRequest;
            }

            throw new ArgumentException("The given XML representation of a UpdateFirmware request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomUpdateFirmwareRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a UpdateFirmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomUpdateFirmwareRequestParser">A delegate to parse custom UpdateFirmware requests.</param>
        public static UpdateFirmwareRequest Parse(JObject                                             JSON,
                                                  Request_Id                                          RequestId,
                                                  ChargeBox_Id                                        ChargeBoxId,
                                                  CustomJObjectParserDelegate<UpdateFirmwareRequest>  CustomUpdateFirmwareRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out UpdateFirmwareRequest  updateFirmwareRequest,
                         out String                 ErrorResponse,
                         CustomUpdateFirmwareRequestParser))
            {
                return updateFirmwareRequest;
            }

            throw new ArgumentException("The given JSON representation of a UpdateFirmware request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of a UpdateFirmware request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UpdateFirmwareRequest Parse(String               Text,
                                                  Request_Id           RequestId,
                                                  ChargeBox_Id         ChargeBoxId,
                                                  OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out UpdateFirmwareRequest updateFirmwareRequest,
                         OnException))
            {
                return updateFirmwareRequest;
            }

            throw new ArgumentException("The given text representation of a UpdateFirmware request is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out UpdateFirmwareRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a UpdateFirmware request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="UpdateFirmwareRequest">The parsed UpdateFirmware request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                   XML,
                                       Request_Id                 RequestId,
                                       ChargeBox_Id               ChargeBoxId,
                                       out UpdateFirmwareRequest  UpdateFirmwareRequest,
                                       OnExceptionDelegate        OnException  = null)
        {

            try
            {

                UpdateFirmwareRequest = new UpdateFirmwareRequest(

                                            ChargeBoxId,

                                            XML.ElementValueOrFail(OCPPNS.OCPPv1_6_CP + "location"),

                                            XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "retrieveDate",
                                                                                        DateTime.Parse),

                                            XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "retries",
                                                                                        Byte.Parse),

                                            XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "retryInterval",
                                                                                        s => TimeSpan.FromSeconds(UInt32.Parse(s))),

                                            RequestId

                                        );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, XML, e);

                UpdateFirmwareRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON,  RequestId, ChargeBoxId, out UpdateFirmwareRequest, out ErrorResponse, CustomUpdateFirmwareRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a UpdateFirmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="UpdateFirmwareRequest">The parsed UpdateFirmware request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                    JSON,
                                       Request_Id                 RequestId,
                                       ChargeBox_Id               ChargeBoxId,
                                       out UpdateFirmwareRequest  UpdateFirmwareRequest,
                                       out String                 ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out UpdateFirmwareRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a UpdateFirmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="UpdateFirmwareRequest">The parsed UpdateFirmware request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUpdateFirmwareRequestParser">A delegate to parse custom UpdateFirmware requests.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       Request_Id                                          RequestId,
                                       ChargeBox_Id                                        ChargeBoxId,
                                       out UpdateFirmwareRequest                           UpdateFirmwareRequest,
                                       out String                                          ErrorResponse,
                                       CustomJObjectParserDelegate<UpdateFirmwareRequest>  CustomUpdateFirmwareRequestParser)
        {

            try
            {

                UpdateFirmwareRequest = null;

                #region Location         [optional]

                if (!JSON.ParseMandatoryText("location",
                                             "location",
                                             out String Location,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region RetrieveDate     [optional]

                if (!JSON.ParseMandatory("retrieveDate",
                                         "retrieve date",
                                         out DateTime RetrieveDate,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Retries          [optional]

                if (JSON.ParseOptional("retries",
                                       "retries",
                                       out Byte? Retries,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region RetryInterval    [optional]

                if (JSON.ParseOptional("retryInterval",
                                       "retry interval",
                                       out TimeSpan? RetryInterval,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region ChargeBoxId      [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                UpdateFirmwareRequest = new UpdateFirmwareRequest(ChargeBoxId,
                                                                  Location,
                                                                  RetrieveDate,
                                                                  Retries,
                                                                  RetryInterval,
                                                                  RequestId);

                if (CustomUpdateFirmwareRequestParser != null)
                    UpdateFirmwareRequest = CustomUpdateFirmwareRequestParser(JSON,
                                                                              UpdateFirmwareRequest);

                return true;

            }
            catch (Exception e)
            {
                UpdateFirmwareRequest  = default;
                ErrorResponse          = "The given JSON representation of a UpdateFirmware request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, RequestId, ChargeBoxId, out UpdateFirmwareRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a UpdateFirmware request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="UpdateFirmwareRequest">The parsed UpdateFirmware request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                     Text,
                                       Request_Id                 RequestId,
                                       ChargeBox_Id               ChargeBoxId,
                                       out UpdateFirmwareRequest  UpdateFirmwareRequest,
                                       OnExceptionDelegate        OnException  = null)
        {

            try
            {

                Text = Text?.Trim();

                if (Text.IsNotNullOrEmpty())
                {

                    if (Text.StartsWith("{") &&
                        TryParse(JObject.Parse(Text),
                                 RequestId,
                                 ChargeBoxId,
                                 out UpdateFirmwareRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(Text).Root,
                                 RequestId,
                                 ChargeBoxId,
                                 out UpdateFirmwareRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, Text, e);
            }

            UpdateFirmwareRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "getDiagnosticsRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "retrieveDate",         RetrieveDate.ToIso8601()),
                   new XElement(OCPPNS.OCPPv1_6_CP + "location",             Location),

                   Retries.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "retries",        Retries.Value)
                       : null,

                   RetryInterval.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "retryInterval",  (UInt64) RetryInterval.Value.TotalSeconds)
                       : null

               );

        #endregion

        #region ToJSON(CustomUpdateFirmwareRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateFirmwareRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateFirmwareRequest> CustomUpdateFirmwareRequestSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("retrieveDate",         RetrieveDate.ToIso8601()),
                           new JProperty("location",             Location),

                           Retries.HasValue
                               ? new JProperty("retries",        Retries.Value)
                               : null,

                           RetryInterval.HasValue
                               ? new JProperty("retryInterval",  (UInt64) RetryInterval.Value.TotalSeconds)
                               : null

                       );

            return CustomUpdateFirmwareRequestSerializer != null
                       ? CustomUpdateFirmwareRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UpdateFirmwareRequest1, UpdateFirmwareRequest2)

        /// <summary>
        /// Compares two UpdateFirmware requests for equality.
        /// </summary>
        /// <param name="UpdateFirmwareRequest1">A UpdateFirmware request.</param>
        /// <param name="UpdateFirmwareRequest2">Another UpdateFirmware request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateFirmwareRequest UpdateFirmwareRequest1, UpdateFirmwareRequest UpdateFirmwareRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateFirmwareRequest1, UpdateFirmwareRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((UpdateFirmwareRequest1 is null) || (UpdateFirmwareRequest2 is null))
                return false;

            return UpdateFirmwareRequest1.Equals(UpdateFirmwareRequest2);

        }

        #endregion

        #region Operator != (UpdateFirmwareRequest1, UpdateFirmwareRequest2)

        /// <summary>
        /// Compares two UpdateFirmware requests for inequality.
        /// </summary>
        /// <param name="UpdateFirmwareRequest1">A UpdateFirmware request.</param>
        /// <param name="UpdateFirmwareRequest2">Another UpdateFirmware request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateFirmwareRequest UpdateFirmwareRequest1, UpdateFirmwareRequest UpdateFirmwareRequest2)

            => !(UpdateFirmwareRequest1 == UpdateFirmwareRequest2);

        #endregion

        #endregion

        #region IEquatable<UpdateFirmwareRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is UpdateFirmwareRequest UpdateFirmwareRequest))
                return false;

            return Equals(UpdateFirmwareRequest);

        }

        #endregion

        #region Equals(UpdateFirmwareRequest)

        /// <summary>
        /// Compares two UpdateFirmware requests for equality.
        /// </summary>
        /// <param name="UpdateFirmwareRequest">A UpdateFirmware request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(UpdateFirmwareRequest UpdateFirmwareRequest)
        {

            if (UpdateFirmwareRequest is null)
                return false;

            return Location.    Equals(UpdateFirmwareRequest.Location) &&
                   RetrieveDate.Equals(UpdateFirmwareRequest.RetrieveDate) &&

                   ((!Retries.      HasValue && !UpdateFirmwareRequest.Retries.      HasValue) ||
                     (Retries.      HasValue &&  UpdateFirmwareRequest.Retries.      HasValue && Retries.      Value.Equals(UpdateFirmwareRequest.Retries.      Value))) &&

                   ((!RetryInterval.HasValue && !UpdateFirmwareRequest.RetryInterval.HasValue) ||
                     (RetryInterval.HasValue &&  UpdateFirmwareRequest.RetryInterval.HasValue && RetryInterval.Value.Equals(UpdateFirmwareRequest.RetryInterval.Value)));

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Location.    GetHashCode() * 11 ^
                       RetrieveDate.GetHashCode() *  7 ^

                       (Retries.HasValue
                            ? Retries.      GetHashCode() * 5
                            : 0) ^

                       (RetryInterval.HasValue
                            ? RetryInterval.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Location,

                             " till ", RetrieveDate,

                             Retries.HasValue
                                 ? ", " + Retries.Value + " retries"
                                 : "",

                             RetryInterval.HasValue
                                 ? ", retry interval " + RetryInterval.Value.TotalSeconds + " sec(s)"
                                 : "");

        #endregion

    }

}
