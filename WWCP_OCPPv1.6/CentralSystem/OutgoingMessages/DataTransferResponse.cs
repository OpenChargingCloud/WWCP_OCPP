/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A data transfer response.
    /// </summary>
    public class DataTransferResponse : AResponse<CS.DataTransferRequest,
                                                  DataTransferResponse>,
                                        IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/dataTransferResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure status of the data transfer.
        /// </summary>
        [Mandatory]
        public DataTransferStatus  Status        { get; }

        /// <summary>
        /// Optional response data.
        /// </summary>
        [Optional]
        public JToken?             Data          { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?         StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region DataTransferResponse(Request, Status, Data = null, StatusInfo = null, ...)

        /// <summary>
        /// Create a new data transfer response.
        /// </summary>
        /// <param name="Request">The data transfer request leading to this response.</param>
        /// <param name="Status">The success or failure status of the data transfer.</param>
        /// <param name="Data">A vendor-specific JSON token.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public DataTransferResponse(CS.DataTransferRequest        Request,
                                    DataTransferStatus            Status,
                                    JToken?                       Data                = null,
                                    StatusInfo?                   StatusInfo          = null,
                                    DateTime?                     ResponseTimestamp   = null,

                                    IEnumerable<KeyPair>?         SignKeys            = null,
                                    IEnumerable<SignInfo>?        SignInfos           = null,
                                    IEnumerable<OCPP.Signature>?  Signatures          = null,

                                    CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status      = Status;
            this.Data        = Data;
            this.StatusInfo  = StatusInfo;

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
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:DataTransferResponse",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     },
        //     "DataTransferStatusEnumType": {
        //       "description": "This indicates the success or failure of the data transfer.\r\n",
        //       "javaType": "DataTransferStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "UnknownMessageId",
        //         "UnknownVendorId"
        //       ]
        //     },
        //     "StatusInfoType": {
        //       "description": "Element providing more information about the status.\r\n",
        //       "javaType": "StatusInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "reasonCode": {
        //           "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "additionalInfo": {
        //           "description": "Additional text to provide detailed information.\r\n",
        //           "type": "string",
        //           "maxLength": 512
        //         }
        //       },
        //       "required": [
        //         "reasonCode"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "status": {
        //       "$ref": "#/definitions/DataTransferStatusEnumType"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     },
        //     "data": {
        //       "description": "Data without specified length or format, in response to request.\r\n"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a data transfer response.
        /// </summary>
        /// <param name="Request">The incoming data transfer request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static DataTransferResponse Parse(CS.DataTransferRequest  Request,
                                                 XElement                XML,
                                                 XNamespace              XMLNamespace)
        {

            if (TryParse(Request,
                         XML,
                         XMLNamespace,
                         out var dataTransferResponse,
                         out var errorResponse) &&
                dataTransferResponse is not null)
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
        /// <param name="CustomDataTransferResponseParser">An optional delegate to parse custom data transfer responses.</param>
        public static DataTransferResponse Parse(CS.DataTransferRequest                              Request,
                                                 JObject                                             JSON,
                                                 CustomJObjectParserDelegate<DataTransferResponse>?  CustomDataTransferResponseParser  = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var dataTransferResponse,
                         out var errorResponse,
                         CustomDataTransferResponseParser) &&
                dataTransferResponse is not null)
            {
                return dataTransferResponse;
            }

            throw new ArgumentException("The given JSON representation of a data transfer response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out DataTransferResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a data transfer response.
        /// </summary
        /// <param name="Request">The incoming data transfer request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="DataTransferResponse">The parsed data transfer response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.DataTransferRequest     Request,
                                       XElement                   XML,
                                       XNamespace                 XMLNamespace,
                                       out DataTransferResponse?  DataTransferResponse,
                                       out String?                ErrorResponse)
        {

            try
            {

                DataTransferResponse = new DataTransferResponse(

                                           Request,

                                           XML.MapEnumValuesOrFail  (XMLNamespace + "status",
                                                                     DataTransferStatusExtensions.Parse),

                                           XML.ElementValueOrDefault(XMLNamespace + "data")

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
        /// <param name="CustomDataTransferResponseParser">An optional delegate to parse custom data transfer responses.</param>
        public static Boolean TryParse(CS.DataTransferRequest                              Request,
                                       JObject                                             JSON,
                                       out DataTransferResponse?                           DataTransferResponse,
                                       out String?                                         ErrorResponse,
                                       CustomJObjectParserDelegate<DataTransferResponse>?  CustomDataTransferResponseParser   = null)
        {

            try
            {

                DataTransferResponse = null;

                #region DataTransferStatus    [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "data transfer status",
                                         DataTransferStatusExtensions.TryParse,
                                         out DataTransferStatus DataTransferStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Data                  [optional]

                var Data = JSON["data"];

                #endregion

                #region StatusInfo            [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPP.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures            [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData            [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                DataTransferResponse = new DataTransferResponse(
                                           Request,
                                           DataTransferStatus,
                                           Data,
                                           StatusInfo,
                                           null,
                                           null,
                                           null,
                                           Signatures,
                                           CustomData
                                       );

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

        #region ToXML (XMLNamespace)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XMLNamespace">The XML namespace to use.</param>
        public XElement ToXML(XNamespace XMLNamespace) // OCPPNS.OCPPv1_6_CP

            => new(XMLNamespace + "dataTransferResponse",

                         new XElement(XMLNamespace + "status",   Status.AsText()),

                   Data is not null && Data.Type == JTokenType.String
                       ? new XElement(XMLNamespace + "data",     Data.  Value<String>())
                       : null

               );

        #endregion

        #region ToJSON(CustomDataTransferResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDataTransferResponseSerializer">A delegate to serialize custom data transfer responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DataTransferResponse>?  CustomDataTransferResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?            CustomStatusInfoSerializer             = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?        CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           Data is not null
                               ? new JProperty("data",         Data)
                               : null,

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
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

               Status.     Equals(DataTransferResponse.Status) &&

             ((Data       is     null && DataTransferResponse.Data       is     null) ||
              (Data       is not null && DataTransferResponse.Data       is not null && Data.      Equals(DataTransferResponse.Data)))      &&

             ((StatusInfo is     null && DataTransferResponse.StatusInfo is     null) ||
               StatusInfo is not null && DataTransferResponse.StatusInfo is not null && StatusInfo.Equals(DataTransferResponse.StatusInfo)) &&

               base.GenericEquals(DataTransferResponse);

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

                return Status.     GetHashCode()       * 7 ^
                      (Data?.      GetHashCode() ?? 0) * 5 ^
                      (StatusInfo?.GetHashCode() ?? 0) * 3 ^
                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(
                   Status,
                   Data is not null
                       ? ", " + Data
                       : ""
               );

        #endregion

    }

}
