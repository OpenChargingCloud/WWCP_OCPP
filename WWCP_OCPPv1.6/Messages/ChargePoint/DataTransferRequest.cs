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
    /// The DataTransfer request.
    /// </summary>
    public class DataTransferRequest : ARequest<DataTransferRequest>
    {

        #region Properties

        /// <summary>
        /// The vendor identification or namespace of the given message.
        /// </summary>
        public String  VendorId     { get; }

        /// <summary>
        /// An optional message identification field.
        /// </summary>
        public String  MessageId    { get; }

        /// <summary>
        /// Optional message data as text without specified length or format.
        /// </summary>
        public String  Data         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DataTransfer request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional message data as text without specified length or format.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public DataTransferRequest(ChargeBox_Id  ChargeBoxId,
                                   String        VendorId,
                                   String        MessageId          = null,
                                   String        Data               = null,

                                   Request_Id?   RequestId          = null,
                                   DateTime?     RequestTimestamp   = null,
                                   EventTracking_Id  EventTrackingId           = null)

            : base(ChargeBoxId,
                   "DataTransfer",
                   RequestId,
                   EventTrackingId,
                   RequestTimestamp)

        {

            this.VendorId   = VendorId?. Trim() ?? throw new ArgumentNullException(nameof(VendorId), "The given vendor identification must not be null or empty!");
            this.MessageId  = MessageId?.Trim();
            this.Data       = Data?.     Trim();

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
        //       <ns:dataTransferRequest>
        //
        //          <ns:vendorId>?</ns:vendorId>
        //
        //          <!--Optional:-->
        //          <ns:messageId>?</ns:messageId>
        //
        //          <!--Optional:-->
        //          <ns:data>?</ns:data>
        //
        //       </ns:dataTransferRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:DataTransferRequest",
        //     "title":   "DataTransferRequest",
        //     "type":    "object",
        //     "properties": {
        //         "vendorId": {
        //             "type": "string",
        //             "maxLength": 255
        //         },
        //         "messageId": {
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "data": {
        //             "type": "string"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "vendorId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of a DataTransfer request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DataTransferRequest Parse(XElement             XML,
                                                Request_Id           RequestId,
                                                ChargeBox_Id         ChargeBoxId,
                                                OnExceptionDelegate  OnException = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out DataTransferRequest dataTransferRequest,
                         OnException))
            {
                return dataTransferRequest;
            }

            throw new ArgumentException("The given XML representation of a DataTransfer request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomDataTransferRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a DataTransfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomDataTransferRequestParser">A delegate to parse custom DataTransfer requests.</param>
        public static DataTransferRequest Parse(JObject                                           JSON,
                                                Request_Id                                        RequestId,
                                                ChargeBox_Id                                      ChargeBoxId,
                                                CustomJObjectParserDelegate<DataTransferRequest>  CustomDataTransferRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out DataTransferRequest  dataTransferRequest,
                         out String               ErrorResponse,
                         CustomDataTransferRequestParser))
            {
                return dataTransferRequest;
            }

            throw new ArgumentException("The given JSON representation of a DataTransfer request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of a DataTransfer request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DataTransferRequest Parse(String               Text,
                                                Request_Id           RequestId,
                                                ChargeBox_Id         ChargeBoxId,
                                                OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out DataTransferRequest dataTransferRequest,
                         OnException))
            {
                return dataTransferRequest;
            }

            throw new ArgumentException("The given text representation of a DataTransfer request is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out DataTransferRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a DataTransfer request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                 XML,
                                       Request_Id               RequestId,
                                       ChargeBox_Id             ChargeBoxId,
                                       out DataTransferRequest  DataTransferRequest,
                                       OnExceptionDelegate      OnException  = null)
        {

            try
            {

                DataTransferRequest = new DataTransferRequest(

                                          ChargeBoxId,

                                          XML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CP + "vendorId"),
                                          XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CP + "messageId"),
                                          XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CP + "data"),

                                          RequestId

                                      );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, XML, e);

                DataTransferRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out DataTransferRequest, out ErrorResponse, CustomDataTransferRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a DataTransfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       Request_Id                                        RequestId,
                                       ChargeBox_Id                                      ChargeBoxId,
                                       out DataTransferRequest                           DataTransferRequest,
                                       out String                                        ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out DataTransferRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a DataTransfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDataTransferRequestParser">A delegate to parse custom DataTransfer requests.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       Request_Id                                        RequestId,
                                       ChargeBox_Id                                      ChargeBoxId,
                                       out DataTransferRequest                           DataTransferRequest,
                                       out String                                        ErrorResponse,
                                       CustomJObjectParserDelegate<DataTransferRequest>  CustomDataTransferRequestParser)
        {

            try
            {

                DataTransferRequest = null;

                #region VendorId        [optional]

                if (!JSON.ParseMandatoryText("vendorId",
                                             "vendor identification",
                                             out String VendorId,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MessageId       [optional]

                var MessageId = JSON.GetString("messageId");

                #endregion

                #region Data            [optional]

                var Data = JSON.GetString("data");

                #endregion

                #region ChargeBoxId     [optional, OCPP_CSE]

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


                DataTransferRequest = new DataTransferRequest(ChargeBoxId,
                                                              VendorId,
                                                              MessageId,
                                                              Data,
                                                              RequestId);

                if (CustomDataTransferRequestParser != null)
                    DataTransferRequest = CustomDataTransferRequestParser(JSON,
                                                                          DataTransferRequest);

                return true;

            }
            catch (Exception e)
            {
                DataTransferRequest  = default;
                ErrorResponse        = "The given JSON representation of a DataTransfer request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, RequestId, ChargeBoxId, out DataTransferRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a DataTransfer request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                   Text,
                                       Request_Id               RequestId,
                                       ChargeBox_Id             ChargeBoxId,
                                       out DataTransferRequest  DataTransferRequest,
                                       OnExceptionDelegate      OnException  = null)
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
                                 out DataTransferRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(Text).Root,
                                 RequestId,
                                 ChargeBoxId,
                                 out DataTransferRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, Text, e);
            }

            DataTransferRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "dataTransferRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "vendorId",         VendorId),

                   MessageId.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "messageId",  MessageId)
                       : null,

                   Data.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "data",       Data)
                       : null

               );

        #endregion

        #region ToJSON(CustomDataTransferRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDataTransferRequestSerializer">A delegate to serialize custom DataTransfer requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DataTransferRequest> CustomDataTransferRequestSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("vendorId",         VendorId),

                           MessageId.IsNotNullOrEmpty()
                               ? new JProperty("messageId",  MessageId)
                               : null,

                           Data.IsNotNullOrEmpty()
                               ? new JProperty("data",       Data)
                               : null

                       );

            return CustomDataTransferRequestSerializer != null
                       ? CustomDataTransferRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DataTransferRequest1, DataTransferRequest2)

        /// <summary>
        /// Compares two DataTransfer requests for equality.
        /// </summary>
        /// <param name="DataTransferRequest1">A DataTransfer request.</param>
        /// <param name="DataTransferRequest2">Another DataTransfer request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DataTransferRequest DataTransferRequest1, DataTransferRequest DataTransferRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DataTransferRequest1, DataTransferRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((DataTransferRequest1 is null) || (DataTransferRequest2 is null))
                return false;

            return DataTransferRequest1.Equals(DataTransferRequest2);

        }

        #endregion

        #region Operator != (DataTransferRequest1, DataTransferRequest2)

        /// <summary>
        /// Compares two DataTransfer requests for inequality.
        /// </summary>
        /// <param name="DataTransferRequest1">A DataTransfer request.</param>
        /// <param name="DataTransferRequest2">Another DataTransfer request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DataTransferRequest DataTransferRequest1, DataTransferRequest DataTransferRequest2)

            => !(DataTransferRequest1 == DataTransferRequest2);

        #endregion

        #endregion

        #region IEquatable<DataTransferRequest> Members

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

            if (!(Object is DataTransferRequest DataTransferRequest))
                return false;

            return Equals(DataTransferRequest);

        }

        #endregion

        #region Equals(DataTransferRequest)

        /// <summary>
        /// Compares two DataTransfer requests for equality.
        /// </summary>
        /// <param name="DataTransferRequest">A DataTransfer request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(DataTransferRequest DataTransferRequest)
        {

            if (DataTransferRequest is null)
                return false;

            return VendorId.Equals(DataTransferRequest.VendorId) &&

                   ((MessageId == null && DataTransferRequest.MessageId == null) ||
                    (MessageId != null && DataTransferRequest.MessageId != null && MessageId.Equals(DataTransferRequest.MessageId))) &&

                   ((Data      == null && DataTransferRequest.Data      == null) ||
                    (Data      != null && DataTransferRequest.Data      != null && Data.     Equals(DataTransferRequest.Data)));

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

                return VendorId.GetHashCode() * 17 ^

                       (MessageId != null
                            ? MessageId.GetHashCode() * 11
                            : 0) ^

                       (Data != null
                            ? Data.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(VendorId, " / ", MessageId, " / ", Data.SubstringMax(20));

        #endregion

    }

}
