/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
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
        [Mandatory]
        public String   VendorId     { get; }

        /// <summary>
        /// An optional message identification field.
        /// </summary>
        [Optional]
        public String?  MessageId    { get; }

        /// <summary>
        /// Optional message data as text without specified length or format.
        /// </summary>
        [Optional]
        public String?  Data         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DataTransfer request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification field.</param>
        /// <param name="Data">Optional message data as text without specified length or format.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public DataTransferRequest(ChargeBox_Id        ChargeBoxId,
                                   String              VendorId,
                                   String?             MessageId                 = null,
                                   String?             Data                      = null,

                                   Request_Id?         RequestId                 = null,
                                   DateTime?           RequestTimestamp          = null,
                                   TimeSpan?           RequestTimeout            = null,
                                   EventTracking_Id?   EventTrackingId           = null,
                                   CancellationToken?  CancellationToken         = null)

            : base(ChargeBoxId,
                   "DataTransfer",
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            #region Initial checks

            this.VendorId = VendorId.Trim();

            if (this.VendorId.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(VendorId), "The given vendor identification must not be null or empty!");

            #endregion

            this.MessageId  = MessageId;
            this.Data       = Data;

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
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
        /// Parse the given XML representation of a data transfer request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DataTransferRequest Parse(XElement              XML,
                                                Request_Id            RequestId,
                                                ChargeBox_Id          ChargeBoxId,
                                                OnExceptionDelegate?  OnException   = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out var dataTransferRequest,
                         OnException))
            {
                return dataTransferRequest!;
            }

            throw new ArgumentException("The given XML representation of a data transfer request is invalid: ", // + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomDataTransferRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a data transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomDataTransferRequestParser">A delegate to parse custom data transfer requests.</param>
        public static DataTransferRequest Parse(JObject                                            JSON,
                                                Request_Id                                         RequestId,
                                                ChargeBox_Id                                       ChargeBoxId,
                                                CustomJObjectParserDelegate<DataTransferRequest>?  CustomDataTransferRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var dataTransferRequest,
                         out var errorResponse,
                         CustomDataTransferRequestParser))
            {
                return dataTransferRequest!;
            }

            throw new ArgumentException("The given JSON representation of a data transfer request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out DataTransferRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a data transfer request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                  XML,
                                       Request_Id                RequestId,
                                       ChargeBox_Id              ChargeBoxId,
                                       out DataTransferRequest?  DataTransferRequest,
                                       OnExceptionDelegate?      OnException   = null)
        {

            try
            {

                DataTransferRequest = new DataTransferRequest(
                                          ChargeBoxId,
                                          XML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "vendorId"),
                                          XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "messageId"),
                                          XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "data"),
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

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out DataTransferRequest, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a data transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                   JSON,
                                       Request_Id                RequestId,
                                       ChargeBox_Id              ChargeBoxId,
                                       out DataTransferRequest?  DataTransferRequest,
                                       out String?               ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out DataTransferRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a data transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDataTransferRequestParser">A delegate to parse custom DataTransfer requests.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       Request_Id                                         RequestId,
                                       ChargeBox_Id                                       ChargeBoxId,
                                       out DataTransferRequest?                           DataTransferRequest,
                                       out String?                                        ErrorResponse,
                                       CustomJObjectParserDelegate<DataTransferRequest>?  CustomDataTransferRequestParser)
        {

            try
            {

                DataTransferRequest = null;

                #region VendorId        [mandatory]

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

                    if (ErrorResponse is not null)
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

                if (CustomDataTransferRequestParser is not null)
                    DataTransferRequest = CustomDataTransferRequestParser(JSON,
                                                                          DataTransferRequest);

                return true;

            }
            catch (Exception e)
            {
                DataTransferRequest  = null;
                ErrorResponse        = "The given JSON representation of a data transfer request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "dataTransferRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "vendorId",         VendorId),

                   MessageId.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "messageId",  MessageId)
                       : null,

                   Data.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "data",       Data)
                       : null

               );

        #endregion

        #region ToJSON(CustomDataTransferSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public override JObject ToJSON()
            => ToJSON(null);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDataTransferSerializer">A delegate to serialize custom DataTransfer requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DataTransferRequest>? CustomDataTransferSerializer)
        {

            var json = JSONObject.Create(

                           new JProperty("vendorId",         VendorId),

                           MessageId.IsNotNullOrEmpty()
                               ? new JProperty("messageId",  MessageId)
                               : null,

                           Data.IsNotNullOrEmpty()
                               ? new JProperty("data",       Data)
                               : null

                       );

            return CustomDataTransferSerializer is not null
                       ? CustomDataTransferSerializer(this, json)
                       : json;

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
        public static Boolean operator == (DataTransferRequest DataTransferRequest1,
                                           DataTransferRequest DataTransferRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DataTransferRequest1, DataTransferRequest2))
                return true;

            // If one is null, but not both, return false.
            if (DataTransferRequest1 is null || DataTransferRequest2 is null)
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
        public static Boolean operator != (DataTransferRequest DataTransferRequest1,
                                           DataTransferRequest DataTransferRequest2)

            => !(DataTransferRequest1 == DataTransferRequest2);

        #endregion

        #endregion

        #region IEquatable<DataTransferRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two data transfer requests for equality.
        /// </summary>
        /// <param name="Object">A data transfer request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DataTransferRequest dataTransferRequest &&
                   Equals(dataTransferRequest);

        #endregion

        #region Equals(DataTransferRequest)

        /// <summary>
        /// Compares two data transfer requests for equality.
        /// </summary>
        /// <param name="DataTransferRequest">A data transfer request to compare with.</param>
        public override Boolean Equals(DataTransferRequest? DataTransferRequest)

            => DataTransferRequest is not null               &&

               VendorId.Equals(DataTransferRequest.VendorId) &&

             ((MessageId is     null && DataTransferRequest.MessageId is     null) ||
              (MessageId is not null && DataTransferRequest.MessageId is not null && MessageId.Equals(DataTransferRequest.MessageId))) &&

             ((Data      is     null && DataTransferRequest.Data      is     null) ||
              (Data      is not null && DataTransferRequest.Data      is not null && Data.     Equals(DataTransferRequest.Data)));

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

                return VendorId.  GetHashCode()       * 5 ^

                      (MessageId?.GetHashCode() ?? 0) * 3 ^
                      (Data?.     GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(
                   VendorId,                      ", ",
                   MessageId              ?? "-", ", ",
                   Data?.SubstringMax(20) ?? ""
               );

        #endregion

    }

}
