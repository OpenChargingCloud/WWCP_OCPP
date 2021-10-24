/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
 * This file is part of WWCP <https://github.com/OpenChargingCloud/WWCP_OCPP>
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
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A data transfer response.
    /// </summary>
    public class DataTransferResponse : AResponse<CP.DataTransferRequest,
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
        public String              Data      { get; }

        #endregion

        #region Constructor(s)

        #region DataTransferResponse(Request, IdTagInfo)

        /// <summary>
        /// Create a new data transfer response.
        /// </summary>
        /// <param name="Request">The incoming data transfer request leading to this response.</param>
        /// <param name="Status">The success or failure status of the data transfer.</param>
        /// <param name="Data">Optional response data.</param>
        public DataTransferResponse(CP.DataTransferRequest  Request,
                                            DataTransferStatus              Status,
                                            String                          Data)

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
        /// <param name="Request">The incoming data transfer request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public DataTransferResponse(CP.DataTransferRequest  Request,
                                            Result                          Result)

            : base(Request,
                   Result)

        {

            this.Status = DataTransferStatus.Unknown;

        }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
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

        #region (static) Parse   (Request, DataTransferResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a data transfer response.
        /// </summary>
        /// <param name="Request">The incoming data transfer request leading to this response.</param>
        /// <param name="DataTransferResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DataTransferResponse Parse(CP.DataTransferRequest  Request,
                                                 XElement                DataTransferResponseXML,
                                                 OnExceptionDelegate     OnException = null)
        {

            if (TryParse(Request,
                         DataTransferResponseXML,
                         out DataTransferResponse dataTransferResponse,
                         OnException))
            {
                return dataTransferResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, DataTransferResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a data transfer response.
        /// </summary>
        /// <param name="Request">The incoming data transfer request leading to this response.</param>
        /// <param name="DataTransferResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DataTransferResponse Parse(CP.DataTransferRequest  Request,
                                                 JObject                 DataTransferResponseJSON,
                                                 OnExceptionDelegate     OnException = null)
        {

            if (TryParse(Request,
                         DataTransferResponseJSON,
                         out DataTransferResponse dataTransferResponse,
                         OnException))
            {
                return dataTransferResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, DataTransferResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a data transfer response.
        /// </summary>
        /// <param name="Request">The incoming data transfer request leading to this response.</param>
        /// <param name="DataTransferResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DataTransferResponse Parse(CP.DataTransferRequest  Request,
                                                 String                  DataTransferResponseText,
                                                 OnExceptionDelegate     OnException = null)
        {

            if (TryParse(Request,
                         DataTransferResponseText,
                         out DataTransferResponse dataTransferResponse,
                         OnException))
            {
                return dataTransferResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, DataTransferResponseXML,  out DataTransferResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a data transfer response.
        /// </summary
        /// <param name="Request">The incoming data transfer request leading to this response.</param>
        /// <param name="DataTransferResponseXML">The XML to be parsed.</param>
        /// <param name="DataTransferResponse">The parsed data transfer response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.DataTransferRequest    Request,
                                       XElement                  DataTransferResponseXML,
                                       out DataTransferResponse  DataTransferResponse,
                                       OnExceptionDelegate       OnException  = null)
        {

            try
            {

                DataTransferResponse = new DataTransferResponse(

                                           Request,

                                           DataTransferResponseXML.MapEnumValuesOrFail  (OCPPNS.OCPPv1_6_CS + "status",
                                                                                         DataTransferStatusExtentions.Parse),

                                           DataTransferResponseXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "data")

                                       );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, DataTransferResponseXML, e);

                DataTransferResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, DataTransferResponseJSON, out DataTransferResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a data transfer response.
        /// </summary
        /// <param name="Request">The incoming data transfer request leading to this response.</param>
        /// <param name="DataTransferResponseJSON">The JSON to be parsed.</param>
        /// <param name="DataTransferResponse">The parsed data transfer response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.DataTransferRequest    Request,
                                       JObject                   DataTransferResponseJSON,
                                       out DataTransferResponse  DataTransferResponse,
                                       OnExceptionDelegate       OnException  = null)
        {

            try
            {

                DataTransferResponse = null;

                #region Status

                if (!DataTransferResponseJSON.MapMandatory("status",
                                                           "data transfer status",
                                                           DataTransferStatusExtentions.Parse,
                                                           out DataTransferStatus DataTransferStatus,
                                                           out String ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Data

                var Data = DataTransferResponseJSON.GetString("data");

                #endregion


                DataTransferResponse = new DataTransferResponse(Request,
                                                                        DataTransferStatus,
                                                                        Data);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, DataTransferResponseJSON, e);

                DataTransferResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, DataTransferResponseText, out DataTransferResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a data transfer response.
        /// </summary>
        /// <param name="Request">The incoming data transfer request leading to this response.</param>
        /// <param name="DataTransferResponseText">The text to be parsed.</param>
        /// <param name="DataTransferResponse">The parsed data transfer response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.DataTransferRequest    Request,
                                       String                    DataTransferResponseText,
                                       out DataTransferResponse  DataTransferResponse,
                                       OnExceptionDelegate       OnException   = null)
        {

            try
            {

                DataTransferResponseText = DataTransferResponseText?.Trim();

                if (DataTransferResponseText.IsNotNullOrEmpty())
                {

                    if (DataTransferResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(DataTransferResponseText),
                                 out DataTransferResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(DataTransferResponseText).Root,
                                 out DataTransferResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, DataTransferResponseText, e);
            }

            DataTransferResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "dataTransferResponse",

                   new XElement(OCPPNS.OCPPv1_6_CS + "status",  Status.AsText()),

                   Data.IsNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "data",  Data)
                       : null

               );

        #endregion

        #region ToJSON(CustomDataTransferResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDataTransferResponseSerializer">A delegate to serialize custom authorize responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DataTransferResponse> CustomDataTransferResponseSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("status",      DataTransferStatusExtentions.AsText(Status)),

                           Data.IsNotNullOrEmpty()
                               ? new JProperty("data",  Data)
                               : null

                       );

            return CustomDataTransferResponseSerializer != null
                       ? CustomDataTransferResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The data transfer failed.
        /// </summary>
        /// <param name="Request">The incoming data transfer request leading to this response.</param>
        public static DataTransferResponse Failed(CP.DataTransferRequest Request)

            => new DataTransferResponse(Request,
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
        public static Boolean operator == (DataTransferResponse DataTransferResponse1, DataTransferResponse DataTransferResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DataTransferResponse1, DataTransferResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) DataTransferResponse1 == null) || ((Object) DataTransferResponse2 == null))
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
        public static Boolean operator != (DataTransferResponse DataTransferResponse1, DataTransferResponse DataTransferResponse2)

            => !(DataTransferResponse1 == DataTransferResponse2);

        #endregion

        #endregion

        #region IEquatable<DataTransferResponse> Members

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

            // Check if the given object is a data transfer response.
            var DataTransferResponse = Object as DataTransferResponse;
            if ((Object) DataTransferResponse == null)
                return false;

            return this.Equals(DataTransferResponse);

        }

        #endregion

        #region Equals(DataTransferResponse)

        /// <summary>
        /// Compares two data transfer responses for equality.
        /// </summary>
        /// <param name="DataTransferResponse">A data transfer response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(DataTransferResponse DataTransferResponse)
        {

            if ((Object) DataTransferResponse == null)
                return false;

            return Status.Equals(DataTransferResponse.Status) &&

                   ((Data == null && DataTransferResponse.Data == null) ||
                    (Data != null && DataTransferResponse.Data != null && Data.Equals(DataTransferResponse.Data)));

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

                return Status.GetHashCode() * 11 ^

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

            => String.Concat(Status, " / ", Data.SubstringMax(20));

        #endregion

    }

}
