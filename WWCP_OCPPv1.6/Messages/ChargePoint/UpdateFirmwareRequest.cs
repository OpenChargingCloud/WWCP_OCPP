/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Illias;

using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP update firmware request.
    /// </summary>
    public class UpdateFirmwareRequest : ARequest<UpdateFirmwareRequest>
    {

        #region Properties

        /// <summary>
        /// The URI where to download the firmware.
        /// </summary>
        public String     Location        { get; }

        /// <summary>
        /// The timestamp after which the charge point must retrieve the
        /// firmware.
        /// </summary>
        public DateTime   RetrieveDate    { get; }

        /// <summary>
        /// The optional number of retries of a charge point for trying to
        /// download the firmware before giving up. If this field is not
        /// present, it is left to the charge point to decide how many times
        /// it wants to retry.
        /// </summary>
        public Byte?      Retries         { get; }

        /// <summary>
        /// The interval after which a retry may be attempted. If this field
        /// is not present, it is left to charge point to decide how long to
        /// wait between attempts.
        /// </summary>
        public TimeSpan?  RetryInterval   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP UpdateFirmwareRequest XML/SOAP request.
        /// </summary>
        /// <param name="Location">The URI where to download the firmware.</param>
        /// <param name="RetrieveDate">The timestamp after which the charge point must retrieve the firmware.</param>
        /// <param name="Retries">The optional number of retries of a charge point for trying to download the firmware before giving up. If this field is not present, it is left to the charge point to decide how many times it wants to retry.</param>
        /// <param name="RetryInterval">The interval after which a retry may be attempted. If this field is not present, it is left to charge point to decide how long to wait between attempts.</param>
        public UpdateFirmwareRequest(String    Location,
                                     DateTime  RetrieveDate,
                                     Byte?     Retries        = null,
                                     TimeSpan? RetryInterval  = null)
        {

            #region Initial checks

            if (Location.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Location),  "The given location must not be null or empty!");

            #endregion

            this.Location       = Location;
            this.RetrieveDate   = RetrieveDate;
            this.Retries        = Retries       ?? new Byte?();
            this.RetryInterval  = RetryInterval ?? new TimeSpan?();

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

        #endregion

        #region (static) Parse   (UpdateFirmwareRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP update firmware request.
        /// </summary>
        /// <param name="UpdateFirmwareRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UpdateFirmwareRequest Parse(XElement             UpdateFirmwareRequestXML,
                                                  OnExceptionDelegate  OnException = null)
        {

            UpdateFirmwareRequest _UpdateFirmwareRequest;

            if (TryParse(UpdateFirmwareRequestXML, out _UpdateFirmwareRequest, OnException))
                return _UpdateFirmwareRequest;

            return null;

        }

        #endregion

        #region (static) Parse   (UpdateFirmwareRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP update firmware request.
        /// </summary>
        /// <param name="UpdateFirmwareRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UpdateFirmwareRequest Parse(String               UpdateFirmwareRequestText,
                                                  OnExceptionDelegate  OnException = null)
        {

            UpdateFirmwareRequest _UpdateFirmwareRequest;

            if (TryParse(UpdateFirmwareRequestText, out _UpdateFirmwareRequest, OnException))
                return _UpdateFirmwareRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(UpdateFirmwareRequestXML,  out UpdateFirmwareRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP update firmware request.
        /// </summary>
        /// <param name="UpdateFirmwareRequestXML">The XML to be parsed.</param>
        /// <param name="UpdateFirmwareRequest">The parsed update firmware request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                   UpdateFirmwareRequestXML,
                                       out UpdateFirmwareRequest  UpdateFirmwareRequest,
                                       OnExceptionDelegate        OnException  = null)
        {

            try
            {

                UpdateFirmwareRequest = new UpdateFirmwareRequest(

                                            UpdateFirmwareRequestXML.ElementValueOrFail(OCPPNS.OCPPv1_6_CP + "location"),

                                            UpdateFirmwareRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "retrieveDate",
                                                                                        DateTime.Parse),

                                            UpdateFirmwareRequestXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "retries",
                                                                                        Byte.Parse),

                                            UpdateFirmwareRequestXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "retryInterval",
                                                                                        s => TimeSpan.FromSeconds(UInt32.Parse(s)))

                                        );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, UpdateFirmwareRequestXML, e);

                UpdateFirmwareRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(UpdateFirmwareRequestText, out UpdateFirmwareRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP update firmware request.
        /// </summary>
        /// <param name="UpdateFirmwareRequestText">The text to be parsed.</param>
        /// <param name="UpdateFirmwareRequest">The parsed update firmware request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                     UpdateFirmwareRequestText,
                                       out UpdateFirmwareRequest  UpdateFirmwareRequest,
                                       OnExceptionDelegate        OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(UpdateFirmwareRequestText).Root.Element(SOAPNS.v1_2.NS.SOAPEnvelope + "Body"),
                             out UpdateFirmwareRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, UpdateFirmwareRequestText, e);
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
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "retryInterval",  RetryInterval.Value.TotalSeconds)
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (UpdateFirmwareRequest1, UpdateFirmwareRequest2)

        /// <summary>
        /// Compares two update firmware requests for equality.
        /// </summary>
        /// <param name="UpdateFirmwareRequest1">A update firmware request.</param>
        /// <param name="UpdateFirmwareRequest2">Another update firmware request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateFirmwareRequest UpdateFirmwareRequest1, UpdateFirmwareRequest UpdateFirmwareRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateFirmwareRequest1, UpdateFirmwareRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) UpdateFirmwareRequest1 == null) || ((Object) UpdateFirmwareRequest2 == null))
                return false;

            return UpdateFirmwareRequest1.Equals(UpdateFirmwareRequest2);

        }

        #endregion

        #region Operator != (UpdateFirmwareRequest1, UpdateFirmwareRequest2)

        /// <summary>
        /// Compares two update firmware requests for inequality.
        /// </summary>
        /// <param name="UpdateFirmwareRequest1">A update firmware request.</param>
        /// <param name="UpdateFirmwareRequest2">Another update firmware request.</param>
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

            if (Object == null)
                return false;

            // Check if the given object is a update firmware request.
            var UpdateFirmwareRequest = Object as UpdateFirmwareRequest;
            if ((Object) UpdateFirmwareRequest == null)
                return false;

            return this.Equals(UpdateFirmwareRequest);

        }

        #endregion

        #region Equals(UpdateFirmwareRequest)

        /// <summary>
        /// Compares two update firmware requests for equality.
        /// </summary>
        /// <param name="UpdateFirmwareRequest">A update firmware request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(UpdateFirmwareRequest UpdateFirmwareRequest)
        {

            if ((Object) UpdateFirmwareRequest == null)
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
