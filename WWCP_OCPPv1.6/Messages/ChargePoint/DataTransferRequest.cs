///*
// * Copyright (c) 2014-2023 GraphDefined GmbH
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using System.Xml.Linq;

//using Newtonsoft.Json.Linq;

//using org.GraphDefined.Vanaheimr.Illias;

//using cloud.charging.open.protocols.OCPP;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv1_6.CP
//{

//    /// <summary>
//    /// The data transfer request.
//    /// </summary>
//    public class DataTransferRequest : ARequest<DataTransferRequest>,
//                                       IRequest
//    {

//        #region Data

//        /// <summary>
//        /// The JSON-LD context of this object.
//        /// </summary>
//        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/dataTransferRequest");

//        #endregion

//        #region Properties

//        /// <summary>
//        /// The JSON-LD context of this object.
//        /// </summary>
//        public JSONLDContext  Context
//            => DefaultJSONLDContext;

//        /// <summary>
//        /// The vendor identification or namespace of the given message.
//        /// </summary>
//        [Mandatory]
//        public String         VendorId     { get; }

//        /// <summary>
//        /// An optional message identification field.
//        /// </summary>
//        [Optional]
//        public String?        MessageId    { get; }

//        /// <summary>
//        /// Optional message data as text without specified length or format.
//        /// </summary>
//        [Optional]
//        public String?        Data         { get; }

//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new data transfer request.
//        /// </summary>
//        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
//        /// 
//        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
//        /// <param name="MessageId">An optional message identification field.</param>
//        /// <param name="Data">Optional message data as text without specified length or format.</param>
//        /// 
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
//        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
//        /// 
//        /// <param name="RequestId">An optional request identification.</param>
//        /// <param name="RequestTimestamp">An optional request timestamp.</param>
//        /// <param name="RequestTimeout">The timeout of this request.</param>
//        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
//        /// <param name="NetworkPath">The network path of the request.</param>
//        /// <param name="CancellationToken">An optional token to cancel this request.</param>
//        public DataTransferRequest(NetworkingNode_Id              NetworkingNodeId,
//                                   String                         VendorId,
//                                   String?                        MessageId           = null,
//                                   String?                        Data                = null,

//                                   IEnumerable<KeyPair>?          SignKeys            = null,
//                                   IEnumerable<SignInfo>?         SignInfos           = null,
//                                   IEnumerable<OCPP.Signature>?   Signatures          = null,

//                                   CustomData?                    CustomData          = null,

//                                   Request_Id?                    RequestId           = null,
//                                   DateTime?                      RequestTimestamp    = null,
//                                   TimeSpan?                      RequestTimeout      = null,
//                                   EventTracking_Id?              EventTrackingId     = null,
//                                   NetworkPath?                   NetworkPath         = null,
//                                   CancellationToken              CancellationToken   = default)

//            : base(NetworkingNodeId,
//                   nameof(DataTransferRequest)[..^7],

//                   SignKeys,
//                   SignInfos,
//                   Signatures,

//                   CustomData,

//                   RequestId,
//                   RequestTimestamp,
//                   RequestTimeout,
//                   EventTrackingId,
//                   NetworkPath,
//                   CancellationToken)

//        {

//            #region Initial checks

//            this.VendorId = VendorId.Trim();

//            if (this.VendorId.IsNullOrEmpty())
//                throw new ArgumentNullException(nameof(VendorId), "The given vendor identification must not be null or empty!");

//            #endregion

//            this.MessageId  = MessageId;
//            this.Data       = Data;

//        }

//        #endregion


//        #region Documentation

//        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
//        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
//        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
//        //
//        //    <soap:Header>
//        //       ...
//        //    </soap:Header>
//        //
//        //    <soap:Body>
//        //       <ns:dataTransferRequest>
//        //
//        //          <ns:vendorId>?</ns:vendorId>
//        //
//        //          <!--Optional:-->
//        //          <ns:messageId>?</ns:messageId>
//        //
//        //          <!--Optional:-->
//        //          <ns:data>?</ns:data>
//        //
//        //       </ns:dataTransferRequest>
//        //    </soap:Body>
//        //
//        // </soap:Envelope>

//        // {
//        //     "$schema": "http://json-schema.org/draft-04/schema#",
//        //     "id":      "urn:OCPP:1.6:2019:12:DataTransferRequest",
//        //     "title":   "DataTransferRequest",
//        //     "type":    "object",
//        //     "properties": {
//        //         "vendorId": {
//        //             "type": "string",
//        //             "maxLength": 255
//        //         },
//        //         "messageId": {
//        //             "type": "string",
//        //             "maxLength": 50
//        //         },
//        //         "data": {
//        //             "type": "string"
//        //         }
//        //     },
//        //     "additionalProperties": false,
//        //     "required": [
//        //         "vendorId"
//        //     ]
//        // }

//        #endregion

//        #region (static) Parse   (XML,  RequestId, NetworkingNodeId, OnException = null)

//        /// <summary>
//        /// Parse the given XML representation of a data transfer request.
//        /// </summary>
//        /// <param name="XML">The XML to be parsed.</param>
//        /// <param name="RequestId">The request identification.</param>
//        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
//        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
//        public static DataTransferRequest Parse(XElement              XML,
//                                                XNamespace                XMLNamespace,
//                                                Request_Id            RequestId,
//                                                NetworkingNode_Id          NetworkingNodeId,
//                                                OnExceptionDelegate?  OnException   = null)
//        {

//            if (TryParse(XML,
//                         RequestId,
//                         NetworkingNodeId,
//                         out var dataTransferRequest,
//                         OnException))
//            {
//                return dataTransferRequest!;
//            }

//            throw new ArgumentException("The given XML representation of a data transfer request is invalid: ", // + errorResponse,
//                                        nameof(XML));

//        }

//        #endregion

//        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomDataTransferRequestParser = null)

//        /// <summary>
//        /// Parse the given JSON representation of a data transfer request.
//        /// </summary>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="RequestId">The request identification.</param>
//        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
//        /// <param name="NetworkPath">The network path of the request.</param>
//        /// <param name="CustomDataTransferRequestParser">A delegate to parse custom data transfer requests.</param>
//        public static DataTransferRequest Parse(JObject                                            JSON,
//                                                Request_Id                                         RequestId,
//                                                NetworkingNode_Id                                  NetworkingNodeId,
//                                                NetworkPath                                        NetworkPath,
//                                                CustomJObjectParserDelegate<DataTransferRequest>?  CustomDataTransferRequestParser   = null)
//        {

//            if (TryParse(JSON,
//                         RequestId,
//                         NetworkingNodeId,
//                         NetworkPath,
//                         out var dataTransferRequest,
//                         out var errorResponse,
//                         CustomDataTransferRequestParser) &&
//                dataTransferRequest is not null)
//            {
//                return dataTransferRequest;
//            }

//            throw new ArgumentException("The given JSON representation of a data transfer request is invalid: " + errorResponse,
//                                        nameof(JSON));

//        }

//        #endregion

//        #region (static) TryParse(XML,  RequestId, NetworkingNodeId, out DataTransferRequest, OnException = null)

//        /// <summary>
//        /// Try to parse the given XML representation of a data transfer request.
//        /// </summary>
//        /// <param name="XML">The XML to be parsed.</param>
//        /// <param name="RequestId">The request identification.</param>
//        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
//        /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
//        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
//        public static Boolean TryParse(XElement                  XML,
//                                       XNamespace                XMLNamespace,
//                                       Request_Id                RequestId,
//                                       NetworkingNode_Id         NetworkingNodeId,
//                                       out DataTransferRequest?  DataTransferRequest,
//                                       OnExceptionDelegate?      OnException   = null)
//        {

//            try
//            {

//                DataTransferRequest = new DataTransferRequest(
//                                          NetworkingNodeId,
//                                          XML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CS + "vendorId"),
//                                          XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "messageId"),
//                                          XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CS + "data"),
//                                          RequestId: RequestId
//                                      );

//                return true;

//            }
//            catch (Exception e)
//            {

//                OnException?.Invoke(Timestamp.Now, XML, e);

//                DataTransferRequest = null;
//                return false;

//            }

//        }

//        #endregion

//        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out DataTransferRequest, OnException = null)

//        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

//        /// <summary>
//        /// Try to parse the given JSON representation of a data transfer request.
//        /// </summary>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="RequestId">The request identification.</param>
//        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
//        /// <param name="NetworkPath">The network path of the request.</param>
//        /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
//        /// <param name="ErrorResponse">An optional error response.</param>
//        public static Boolean TryParse(JObject                   JSON,
//                                       Request_Id                RequestId,
//                                       NetworkingNode_Id         NetworkingNodeId,
//                                       NetworkPath               NetworkPath,
//                                       out DataTransferRequest?  DataTransferRequest,
//                                       out String?               ErrorResponse)

//            => TryParse(JSON,
//                        RequestId,
//                        NetworkingNodeId,
//                        NetworkPath,
//                        out DataTransferRequest,
//                        out ErrorResponse,
//                        null);


//        /// <summary>
//        /// Try to parse the given JSON representation of a data transfer request.
//        /// </summary>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="RequestId">The request identification.</param>
//        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
//        /// <param name="NetworkPath">The network path of the request.</param>
//        /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
//        /// <param name="ErrorResponse">An optional error response.</param>
//        /// <param name="CustomDataTransferRequestParser">A delegate to parse custom DataTransfer requests.</param>
//        public static Boolean TryParse(JObject                                            JSON,
//                                       Request_Id                                         RequestId,
//                                       NetworkingNode_Id                                  NetworkingNodeId,
//                                       NetworkPath                                        NetworkPath,
//                                       out DataTransferRequest?                           DataTransferRequest,
//                                       out String?                                        ErrorResponse,
//                                       CustomJObjectParserDelegate<DataTransferRequest>?  CustomDataTransferRequestParser)
//        {

//            try
//            {

//                DataTransferRequest = null;

//                #region VendorId        [mandatory]

//                if (!JSON.ParseMandatoryText("vendorId",
//                                             "vendor identification",
//                                             out String VendorId,
//                                             out ErrorResponse))
//                {
//                    return false;
//                }

//                #endregion

//                #region MessageId       [optional]

//                var MessageId = JSON.GetString("messageId");

//                #endregion

//                #region Data            [optional]

//                var Data = JSON.GetString("data");

//                #endregion

//                #region Signatures      [optional, OCPP_CSE]

//                if (JSON.ParseOptionalHashSet("signatures",
//                                              "cryptographic signatures",
//                                              OCPP.Signature.TryParse,
//                                              out HashSet<OCPP.Signature> Signatures,
//                                              out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion

//                #region CustomData      [optional]

//                if (JSON.ParseOptionalJSON("customData",
//                                           "custom data",
//                                           OCPP.CustomData.TryParse,
//                                           out CustomData CustomData,
//                                           out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion


//                DataTransferRequest = new DataTransferRequest(

//                                          NetworkingNodeId,
//                                          VendorId,
//                                          MessageId,
//                                          Data,

//                                          null,
//                                          null,
//                                          Signatures,

//                                          CustomData,

//                                          RequestId,
//                                          null,
//                                          null,
//                                          null,
//                                          NetworkPath

//                                      );

//                if (CustomDataTransferRequestParser is not null)
//                    DataTransferRequest = CustomDataTransferRequestParser(JSON,
//                                                                          DataTransferRequest);

//                return true;

//            }
//            catch (Exception e)
//            {
//                DataTransferRequest  = null;
//                ErrorResponse        = "The given JSON representation of a data transfer request is invalid: " + e.Message;
//                return false;
//            }

//        }

//        #endregion

//        #region ToXML()

//        /// <summary>
//        /// Return a XML representation of this object.
//        /// </summary>
//        public XElement ToXML()

//            => new (OCPPNS.OCPPv1_6_CS + "dataTransferRequest",

//                   new XElement(OCPPNS.OCPPv1_6_CS + "vendorId",         VendorId),

//                   MessageId.IsNotNullOrEmpty()
//                       ? new XElement(OCPPNS.OCPPv1_6_CS + "messageId",  MessageId)
//                       : null,

//                   Data.IsNotNullOrEmpty()
//                       ? new XElement(OCPPNS.OCPPv1_6_CS + "data",       Data)
//                       : null

//               );

//        #endregion

//        #region ToJSON(CustomDataTransferSerializer = null, CustomSignatureSerializer = null, ...)

//        /// <summary>
//        /// Return a JSON representation of this object.
//        /// </summary>
//        /// <param name="CustomDataTransferSerializer">A delegate to serialize custom DataTransfer requests.</param>
//        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
//        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
//        public JObject ToJSON(CustomJObjectSerializerDelegate<DataTransferRequest>?  CustomDataTransferSerializer   = null,
//                              CustomJObjectSerializerDelegate<OCPP.Signature>?       CustomSignatureSerializer      = null,
//                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer     = null)
//        {

//            var json = JSONObject.Create(

//                                 new JProperty("vendorId",      VendorId),

//                           MessageId.IsNotNullOrEmpty()
//                               ? new JProperty("messageId",     MessageId)
//                               : null,

//                           Data.IsNotNullOrEmpty()
//                               ? new JProperty("data",          Data)
//                               : null,

//                           Signatures.Any()
//                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
//                                                                                                                           CustomCustomDataSerializer))))
//                               : null,

//                           CustomData is not null
//                               ? new JProperty("customData",    CustomData.ToJSON(CustomCustomDataSerializer))
//                               : null

//                       );

//            return CustomDataTransferSerializer is not null
//                       ? CustomDataTransferSerializer(this, json)
//                       : json;

//        }

//        #endregion


//        #region Operator overloading

//        #region Operator == (DataTransferRequest1, DataTransferRequest2)

//        /// <summary>
//        /// Compares two DataTransfer requests for equality.
//        /// </summary>
//        /// <param name="DataTransferRequest1">A DataTransfer request.</param>
//        /// <param name="DataTransferRequest2">Another DataTransfer request.</param>
//        /// <returns>True if both match; False otherwise.</returns>
//        public static Boolean operator == (DataTransferRequest? DataTransferRequest1,
//                                           DataTransferRequest? DataTransferRequest2)
//        {

//            // If both are null, or both are same instance, return true.
//            if (ReferenceEquals(DataTransferRequest1, DataTransferRequest2))
//                return true;

//            // If one is null, but not both, return false.
//            if (DataTransferRequest1 is null || DataTransferRequest2 is null)
//                return false;

//            return DataTransferRequest1.Equals(DataTransferRequest2);

//        }

//        #endregion

//        #region Operator != (DataTransferRequest1, DataTransferRequest2)

//        /// <summary>
//        /// Compares two DataTransfer requests for inequality.
//        /// </summary>
//        /// <param name="DataTransferRequest1">A DataTransfer request.</param>
//        /// <param name="DataTransferRequest2">Another DataTransfer request.</param>
//        /// <returns>False if both match; True otherwise.</returns>
//        public static Boolean operator != (DataTransferRequest? DataTransferRequest1,
//                                           DataTransferRequest? DataTransferRequest2)

//            => !(DataTransferRequest1 == DataTransferRequest2);

//        #endregion

//        #endregion

//        #region IEquatable<DataTransferRequest> Members

//        #region Equals(Object)

//        /// <summary>
//        /// Compares two data transfer requests for equality.
//        /// </summary>
//        /// <param name="Object">A data transfer request to compare with.</param>
//        public override Boolean Equals(Object? Object)

//            => Object is DataTransferRequest dataTransferRequest &&
//                   Equals(dataTransferRequest);

//        #endregion

//        #region Equals(DataTransferRequest)

//        /// <summary>
//        /// Compares two data transfer requests for equality.
//        /// </summary>
//        /// <param name="DataTransferRequest">A data transfer request to compare with.</param>
//        public override Boolean Equals(DataTransferRequest? DataTransferRequest)

//            => DataTransferRequest is not null               &&

//               VendorId.Equals(DataTransferRequest.VendorId) &&

//             ((MessageId is     null && DataTransferRequest.MessageId is     null) ||
//              (MessageId is not null && DataTransferRequest.MessageId is not null && MessageId.Equals(DataTransferRequest.MessageId))) &&

//             ((Data      is     null && DataTransferRequest.Data      is     null) ||
//              (Data      is not null && DataTransferRequest.Data      is not null && Data.     Equals(DataTransferRequest.Data)))      &&

//               base.GenericEquals(DataTransferRequest);

//        #endregion

//        #endregion

//        #region (override) GetHashCode()

//        /// <summary>
//        /// Return the HashCode of this object.
//        /// </summary>
//        /// <returns>The HashCode of this object.</returns>
//        public override Int32 GetHashCode()
//        {
//            unchecked
//            {

//                return VendorId.  GetHashCode()       * 7 ^

//                      (MessageId?.GetHashCode() ?? 0) * 5 ^
//                      (Data?.     GetHashCode() ?? 0) * 3 ^

//                       base.      GetHashCode();

//            }
//        }

//        #endregion

//        #region (override) ToString()

//        /// <summary>
//        /// Return a text representation of this object.
//        /// </summary>
//        public override String ToString()

//            => String.Concat(
//                   VendorId,                      ", ",
//                   MessageId              ?? "-", ", ",
//                   Data?.SubstringMax(20) ?? ""
//               );

//        #endregion

//    }

//}
