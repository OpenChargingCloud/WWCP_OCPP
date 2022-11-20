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
    /// A data transfer response.
    /// </summary>
    public class DataTransferResponse : AResponse<CS.DataTransferRequest,
                                                     DataTransferResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure status of the data transfer.
        /// </summary>
        public DataTransferStatus  Status    { get; }

        /// <summary>
        /// Optional response data.
        /// </summary>
        public String?             Data      { get; }

        #endregion

        #region Constructor(s)

        #region DataTransferResponse(Request, Status, Data = null)

        /// <summary>
        /// Create a new data transfer response.
        /// </summary>
        /// <param name="Request">The data transfer request leading to this response.</param>
        /// <param name="Status">The success or failure status of the data transfer.</param>
        /// <param name="Data">Optional response data.</param>
        public DataTransferResponse(CS.DataTransferRequest  Request,
                                    DataTransferStatus      Status,
                                    String?                 Data   = null)

            : base(Request,
                   Result.OK())

        {

            this.Status  = Status;
            this.Data    = Data;

        }

        #endregion

        #region DataTransferResponse(Request, Result)

        /// <summary>
        /// Create a new data transfer response.
        /// </summary>
        /// <param name="Request">The data transfer request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public DataTransferResponse(CS.DataTransferRequest  Request,
                                    Result                  Result)

            : base(Request,
                   Result)

        {

            this.Status = DataTransferStatus.Unknown;

        }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:dataTransferResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //          <!--Optional:-->
        //          <ns:data>?</ns:data>
        //
        //       </ns:dataTransferResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:DataTransferResponse",
        //     "title":   "DataTransferResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
        //                 "UnknownMessageId",
        //                 "UnknownVendorId"
        //             ]
        //         },
        //         "data": {
        //             "type": "string"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a data transfer response.
        /// </summary>
        /// <param name="Request">The data transfer request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static DataTransferResponse Parse(CS.DataTransferRequest  Request,
                                                 XElement                XML)
        {

            if (TryParse(Request,
                         XML,
                         out var dataTransferResponse,
                         out var errorResponse))
            {
                return dataTransferResponse!;
            }

            throw new ArgumentException("The given XML representation of a data transfer response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomDataTransferResponseSerializer = null)

        /// <summary>
        /// Parse the given JSON representation of a data transfer response.
        /// </summary>
        /// <param name="Request">The data transfer request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDataTransferResponseParser">A delegate to parse custom data transfer responses.</param>
        public static DataTransferResponse Parse(CS.DataTransferRequest                              Request,
                                                 JObject                                             JSON,
                                                 CustomJObjectParserDelegate<DataTransferResponse>?  CustomDataTransferResponseParser  = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var dataTransferResponse,
                         out var errorResponse,
                         CustomDataTransferResponseParser))
            {
                return dataTransferResponse!;
            }

            throw new ArgumentException("The given JSON representation of a data transfer response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out DataTransferResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a data transfer response.
        /// </summary>
        /// <param name="Request">The data transfer request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="DataTransferResponse">The parsed data transfer response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.DataTransferRequest     Request,
                                       XElement                   XML,
                                       out DataTransferResponse?  DataTransferResponse,
                                       out String?                ErrorResponse)
        {

            try
            {

                DataTransferResponse = new DataTransferResponse(

                                           Request,

                                           XML.MapEnumValuesOrFail  (OCPPNS.OCPPv1_6_CP + "status",
                                                                     DataTransferStatusExtentions.Parse),

                                           XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CP + "data")

                                       );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                DataTransferResponse  = null;
                ErrorResponse         = "The given XML representation of a data transfer response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out DataTransferResponse, out ErrorResponse, CustomDataTransferResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a data transfer response.
        /// </summary>
        /// <param name="Request">The data transfer request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DataTransferResponse">The parsed data transfer response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDataTransferResponseParser">A delegate to parse custom data transfer responses.</param>
        public static Boolean TryParse(CS.DataTransferRequest                              Request,
                                       JObject                                             JSON,
                                       out DataTransferResponse?                           DataTransferResponse,
                                       out String?                                         ErrorResponse,
                                       CustomJObjectParserDelegate<DataTransferResponse>?  CustomDataTransferResponseParser   = null)
        {

            try
            {

                DataTransferResponse = null;

                #region DataTransferStatus

                if (!JSON.MapMandatory("status",
                                       "data transfer status",
                                       DataTransferStatusExtentions.Parse,
                                       out DataTransferStatus DataTransferStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Data

                var Data = JSON.GetString("data")?.Trim();

                #endregion


                DataTransferResponse = new DataTransferResponse(Request,
                                                                DataTransferStatus,
                                                                Data);

                if (CustomDataTransferResponseParser is not null)
                    DataTransferResponse = CustomDataTransferResponseParser(JSON,
                                                                            DataTransferResponse);

                return true;

            }
            catch (Exception e)
            {
                DataTransferResponse  = null;
                ErrorResponse         = "The given JSON representation of a data transfer response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "dataTransferResponse",

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",      Status.AsText()),

                   Data.IsNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "data",  Data)
                       : null

               );

        #endregion

        #region ToJSON(CustomDataTransferResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDataTransferResponseSerializer">A delegate to serialize custom data transfer responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DataTransferResponse>?  CustomDataTransferResponseSerializer  = null)
        {

            var json = JSONObject.Create(

                           new JProperty("status",  Status.AsText()),

                           Data.IsNotNullOrEmpty()
                               ? new JProperty("data", Data)
                               : null

                       );

            return CustomDataTransferResponseSerializer is not null
                       ? CustomDataTransferResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The data transfer failed.
        /// </summary>
        /// <param name="Request">The data transfer request leading to this response.</param>
        public static DataTransferResponse Failed(CS.DataTransferRequest  Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (DataTransferResponse1, DataTransferResponse2)

        /// <summary>
        /// Compares two data transfer responses for equality.
        /// </summary>
        /// <param name="DataTransferResponse1">A data transfer response.</param>
        /// <param name="DataTransferResponse2">Another data transfer response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DataTransferResponse? DataTransferResponse1,
                                           DataTransferResponse? DataTransferResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DataTransferResponse1, DataTransferResponse2))
                return true;

            // If one is null, but not both, return false.
            if (DataTransferResponse1 is null || DataTransferResponse2 is null)
                return false;

            return DataTransferResponse1.Equals(DataTransferResponse2);

        }

        #endregion

        #region Operator != (DataTransferResponse1, DataTransferResponse2)

        /// <summary>
        /// Compares two data transfer responses for inequality.
        /// </summary>
        /// <param name="DataTransferResponse1">A data transfer response.</param>
        /// <param name="DataTransferResponse2">Another data transfer response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DataTransferResponse? DataTransferResponse1,
                                           DataTransferResponse? DataTransferResponse2)

            => !(DataTransferResponse1 == DataTransferResponse2);

        #endregion

        #endregion

        #region IEquatable<DataTransferResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two data transfer responses for equality.
        /// </summary>
        /// <param name="Object">A data transfer response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DataTransferResponse dataTransferResponse &&
                   Equals(dataTransferResponse);

        #endregion

        #region Equals(DataTransferResponse)

        /// <summary>
        /// Compares two data transfer responses for equality.
        /// </summary>
        /// <param name="DataTransferResponse">A data transfer response to compare with.</param>
        public override Boolean Equals(DataTransferResponse? DataTransferResponse)

            => DataTransferResponse is not null &&

               Status.Equals(DataTransferResponse.Status) &&

             ((Data is     null && DataTransferResponse.Data is     null) ||
              (Data is not null && DataTransferResponse.Data is not null && Data.Equals(DataTransferResponse.Data)));

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

                return Status.GetHashCode() * 3 ^
                      (Data?. GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Status,
                             Data is not null
                                 ? ", " + Data.SubstringMax(20)
                                 : "");

        #endregion

    }

}
