﻿/*
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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// The DataTransfer request.
    /// </summary>
    public class DataTransferRequest : ARequest<DataTransferRequest>,
                                       IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/dataTransferRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The vendor identification or namespace of the given message.
        /// </summary>
        [Mandatory]
        public Vendor_Id      VendorId     { get; }

        /// <summary>
        /// An optional message identification field.
        /// </summary>
        [Optional]
        public Message_Id?    MessageId    { get; }

        /// <summary>
        /// Optional message data without specified length or format.
        /// </summary>
        [Optional]
        public JToken?        Data         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DataTransfer request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification.</param>
        /// <param name="Data">Optional vendor-specific data (a JSON token).</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public DataTransferRequest(SourceRouting            Destination,
                                   Vendor_Id                VendorId,
                                   Message_Id?              MessageId             = null,
                                   JToken?                  Data                  = null,

                                   IEnumerable<KeyPair>?    SignKeys              = null,
                                   IEnumerable<SignInfo>?   SignInfos             = null,
                                   IEnumerable<Signature>?  Signatures            = null,

                                   Request_Id?              RequestId             = null,
                                   DateTime?                RequestTimestamp      = null,
                                   TimeSpan?                RequestTimeout        = null,
                                   EventTracking_Id?        EventTrackingId       = null,
                                   NetworkPath?             NetworkPath           = null,
                                   SerializationFormats?    SerializationFormat   = null,
                                   CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(DataTransferRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   null,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.VendorId   = VendorId;
            this.MessageId  = MessageId;
            this.Data       = Data;

            unchecked
            {

                hashCode = this.VendorId.  GetHashCode()       * 7 ^
                          (this.MessageId?.GetHashCode() ?? 0) * 5 ^
                          (this.Data?.     GetHashCode() ?? 0) * 3 ^
                           base.           GetHashCode();

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
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:DataTransferRequest",
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
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "messageId": {
        //       "description": "May be used to indicate a specific message or implementation.\r\n",
        //       "type": "string",
        //       "maxLength": 50
        //     },
        //     "data": {
        //       "description": "Data without specified length or format. This needs to be decided by both parties (Open to implementation).\r\n"
        //     },
        //     "vendorId": {
        //       "description": "This identifies the Vendor specific implementation\r\n\r\n",
        //       "type": "string",
        //       "maxLength": 255
        //     }
        //   },
        //   "required": [
        //     "vendorId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (XML, XMLNamespace, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given XML representation of a DataTransfer request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="XMLNamespace">The XML namespace to use.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        public static DataTransferRequest Parse(XElement           XML,
                                                XNamespace         XMLNamespace,
                                                Request_Id         RequestId,
                                                SourceRouting      Destination,
                                                NetworkPath        NetworkPath,
                                                DateTime?          RequestTimestamp   = null,
                                                TimeSpan?          RequestTimeout     = null,
                                                EventTracking_Id?  EventTrackingId    = null)
        {

            if (TryParse(XML,
                         XMLNamespace,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var dataTransferRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId))
            {
                return dataTransferRequest;
            }

            throw new ArgumentException("The given XML representation of a DataTransfer request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON,              RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a DataTransfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomDataTransferRequestParser">A delegate to parse custom DataTransfer requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static DataTransferRequest Parse(JObject                                            JSON,
                                                Request_Id                                         RequestId,
                                                SourceRouting                                      Destination,
                                                NetworkPath                                        NetworkPath,
                                                DateTime?                                          RequestTimestamp                  = null,
                                                TimeSpan?                                          RequestTimeout                    = null,
                                                EventTracking_Id?                                  EventTrackingId                   = null,
                                                CustomJObjectParserDelegate<DataTransferRequest>?  CustomDataTransferRequestParser   = null,
                                                CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                                CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var dataTransferRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomDataTransferRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return dataTransferRequest;
            }

            throw new ArgumentException("The given JSON representation of a DataTransfer request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML, XMLNamespace, RequestId, Destination, NetworkPath, out DataTransferRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given XML representation of a DataTransfer request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="XMLNamespace">The XML namespace to use.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="DataTransferRequest">The parsed BootNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        public static Boolean TryParse(XElement                                       XML,
                                       XNamespace                                     XMLNamespace,
                                       Request_Id                                     RequestId,
                                       SourceRouting                                  Destination,
                                       NetworkPath                                    NetworkPath,
                                       [NotNullWhen(true)]  out DataTransferRequest?  DataTransferRequest,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       DateTime?                                      RequestTimestamp   = null,
                                       TimeSpan?                                      RequestTimeout     = null,
                                       EventTracking_Id?                              EventTrackingId    = null)
        {

            try
            {

                DataTransferRequest = new DataTransferRequest(
                                          Destination,
                                          Vendor_Id. Parse(XML.ElementValueOrFail   (XMLNamespace + "vendorId")),
                                          Message_Id.Parse(XML.ElementValueOrDefault(XMLNamespace + "messageId")),
                                          XML.ElementValueOrDefault(XMLNamespace + "data"),
                                          RequestId:    RequestId,
                                          NetworkPath:  NetworkPath
                                      );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                DataTransferRequest  = null;
                ErrorResponse        = "The given XML representation of a DataTransfer request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON,              RequestId, Destination, NetworkPath, out DataTransferRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a DataTransfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomDataTransferRequestParser">A delegate to parse custom DataTransfer requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       Request_Id                                         RequestId,
                                       SourceRouting                                      Destination,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out DataTransferRequest?      DataTransferRequest,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTime?                                          RequestTimestamp                  = null,
                                       TimeSpan?                                          RequestTimeout                    = null,
                                       EventTracking_Id?                                  EventTrackingId                   = null,
                                       CustomJObjectParserDelegate<DataTransferRequest>?  CustomDataTransferRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                       CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            try
            {

                DataTransferRequest = null;

                #region VendorId      [mandatory]

                if (!JSON.ParseMandatory("vendorId",
                                         "vendor identification",
                                         Vendor_Id.TryParse,
                                         out Vendor_Id VendorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MessageId     [optional]

                if (JSON.ParseOptional("messageId",
                                       "message identification",
                                       Message_Id.TryParse,
                                       out Message_Id? MessageId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Data          [optional]

                var Data = JSON["data"];

                #endregion

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                DataTransferRequest = new DataTransferRequest(

                                          Destination,
                                          VendorId,
                                          MessageId,
                                          Data,

                                          null,
                                          null,
                                          Signatures,

                                          RequestId,
                                          RequestTimestamp,
                                          RequestTimeout,
                                          EventTrackingId,
                                          NetworkPath

                                      );

                if (CustomDataTransferRequestParser is not null)
                    DataTransferRequest = CustomDataTransferRequestParser(JSON,
                                                                          DataTransferRequest);

                return true;

            }
            catch (Exception e)
            {
                DataTransferRequest  = null;
                ErrorResponse        = "The given JSON representation of a DataTransfer request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML (XMLNamespace)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XMLNamespace">The XML namespace to use.</param>
        public XElement ToXML(XNamespace XMLNamespace) // OCPPNS.OCPPv1_6_CS

            => new (XMLNamespace + "dataTransferRequest",

                         new XElement(XMLNamespace + "vendorId",    VendorId),

                   MessageId.IsNotNullOrEmpty()
                       ? new XElement(XMLNamespace + "messageId",   MessageId)
                       : null,

                   Data is not null && Data.Type == JTokenType.String
                       ? new XElement(XMLNamespace + "data",        Data.Value<String>())
                       : null

               );

        #endregion

        #region ToJSON(CustomDataTransferRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDataTransferRequestSerializer">A delegate to serialize custom DataTransfer requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                IncludeJSONLDContext                  = false,
                              CustomJObjectSerializerDelegate<DataTransferRequest>?  CustomDataTransferRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("vendorId",     VendorId.       TextId),

                           MessageId.HasValue
                               ? new JProperty("messageId",    MessageId.Value.TextId)
                               : null,

                           Data is not null
                               ? new JProperty("data",         Data)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDataTransferRequestSerializer is not null
                       ? CustomDataTransferRequestSerializer(this, json)
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
        public static Boolean operator == (DataTransferRequest? DataTransferRequest1,
                                           DataTransferRequest? DataTransferRequest2)
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
        public static Boolean operator != (DataTransferRequest? DataTransferRequest1,
                                           DataTransferRequest? DataTransferRequest2)

            => !(DataTransferRequest1 == DataTransferRequest2);

        #endregion

        #endregion

        #region IEquatable<DataTransferRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DataTransfer requests for equality.
        /// </summary>
        /// <param name="Object">A DataTransfer request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DataTransferRequest dataTransferRequest &&
                   Equals(dataTransferRequest);

        #endregion

        #region Equals(DataTransferRequest)

        /// <summary>
        /// Compares two DataTransfer requests for equality.
        /// </summary>
        /// <param name="DataTransferRequest">A DataTransfer request to compare with.</param>
        public override Boolean Equals(DataTransferRequest? DataTransferRequest)

            => DataTransferRequest is not null               &&

               VendorId.Equals(DataTransferRequest.VendorId) &&

             ((MessageId is     null && DataTransferRequest.MessageId is     null) ||
              (MessageId is not null && DataTransferRequest.MessageId is not null && MessageId.Equals(DataTransferRequest.MessageId))) &&

             ((Data      is     null && DataTransferRequest.Data      is     null) ||
              (Data      is not null && DataTransferRequest.Data      is not null && Data.     Equals(DataTransferRequest.Data)))      &&

               base.GenericEquals(DataTransferRequest);

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

                   $"'{VendorId}'",

                   MessageId.IsNotNullOrEmpty()
                       ? $" / '{MessageId}'"
                       : "",

                   Data is not null
                       ? $" => '{Data}'"
                       : ""

               );

        #endregion


    }


    ///// <summary>
    ///// The DataTransfer request.
    ///// </summary>
    //public class DataTransferRequest<TRequest> : ARequest<DataTransferRequest<TRequest>>,
    //                                             IRequest
    //{

    //    #region Data

    //    /// <summary>
    //    /// The JSON-LD context of this object.
    //    /// </summary>
    //    public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/dataTransferRequest");

    //    #endregion

    //    #region Properties

    //    /// <summary>
    //    /// The JSON-LD context of this object.
    //    /// </summary>
    //    public JSONLDContext  Context
    //        => DefaultJSONLDContext;

    //    /// <summary>
    //    /// The vendor identification or namespace of the given message.
    //    /// </summary>
    //    [Mandatory]
    //    public Vendor_Id      VendorId     { get; }

    //    /// <summary>
    //    /// An optional message identification field.
    //    /// </summary>
    //    [Optional]
    //    public Message_Id?    MessageId    { get; }

    //    /// <summary>
    //    /// Optional message data without specified length or format.
    //    /// </summary>
    //    [Optional]
    //    public JToken?        Data         { get; }

    //    #endregion

    //    #region Constructor(s)

    //    /// <summary>
    //    /// Create a new DataTransfer request.
    //    /// </summary>
    //    /// <param name="Destination">The destination networking node identification or source routing path.</param>
    //    /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
    //    /// <param name="MessageId">An optional message identification.</param>
    //    /// <param name="Data">Optional vendor-specific data (a JSON token).</param>
    //    /// 
    //    /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
    //    /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
    //    /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
    //    /// 
    //    /// <param name="RequestId">An optional request identification.</param>
    //    /// <param name="RequestTimestamp">An optional request timestamp.</param>
    //    /// <param name="RequestTimeout">The timeout of this request.</param>
    //    /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
    //    /// <param name="NetworkPath">The network path of the request.</param>
    //    /// <param name="SerializationFormat">The optional serialization format for this request.</param>
    //    /// <param name="CancellationToken">An optional token to cancel this request.</param>
    //    public DataTransferRequest(SourceRouting            Destination,
    //                               Vendor_Id                VendorId,
    //                               Message_Id?              MessageId             = null,
    //                               JToken?                  Data                  = null,

    //                               IEnumerable<KeyPair>?    SignKeys              = null,
    //                               IEnumerable<SignInfo>?   SignInfos             = null,
    //                               IEnumerable<Signature>?  Signatures            = null,

    //                               Request_Id?              RequestId             = null,
    //                               DateTime?                RequestTimestamp      = null,
    //                               TimeSpan?                RequestTimeout        = null,
    //                               EventTracking_Id?        EventTrackingId       = null,
    //                               NetworkPath?             NetworkPath           = null,
    //                               SerializationFormats?    SerializationFormat   = null,
    //                               CancellationToken        CancellationToken     = default)

    //        : base(Destination,
    //               nameof(DataTransferRequest)[..^7],

    //               SignKeys,
    //               SignInfos,
    //               Signatures,

    //               null,

    //               RequestId,
    //               RequestTimestamp,
    //               RequestTimeout,
    //               EventTrackingId,
    //               NetworkPath,
    //               SerializationFormat ?? SerializationFormats.JSON,
    //               CancellationToken)

    //    {

    //        this.VendorId   = VendorId;
    //        this.MessageId  = MessageId;
    //        this.Data       = Data;

    //        unchecked
    //        {

    //            hashCode = this.VendorId.  GetHashCode()       * 7 ^
    //                      (this.MessageId?.GetHashCode() ?? 0) * 5 ^
    //                      (this.Data?.     GetHashCode() ?? 0) * 3 ^
    //                       base.           GetHashCode();

    //        }

    //    }

    //    #endregion


    //    #region Documentation

    //    // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
    //    //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
    //    //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
    //    //
    //    //    <soap:Header>
    //    //       ...
    //    //    </soap:Header>
    //    //
    //    //    <soap:Body>
    //    //       <ns:dataTransferRequest>
    //    //
    //    //          <ns:vendorId>?</ns:vendorId>
    //    //
    //    //          <!--Optional:-->
    //    //          <ns:messageId>?</ns:messageId>
    //    //
    //    //          <!--Optional:-->
    //    //          <ns:data>?</ns:data>
    //    //
    //    //       </ns:dataTransferRequest>
    //    //    </soap:Body>
    //    //
    //    // </soap:Envelope>

    //    // {
    //    //   "$schema": "http://json-schema.org/draft-06/schema#",
    //    //   "$id": "urn:OCPP:Cp:2:2020:3:DataTransferRequest",
    //    //   "comment": "OCPP 2.0.1 FINAL",
    //    //   "definitions": {
    //    //     "CustomDataType": {
    //    //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
    //    //       "javaType": "CustomData",
    //    //       "type": "object",
    //    //       "properties": {
    //    //         "vendorId": {
    //    //           "type": "string",
    //    //           "maxLength": 255
    //    //         }
    //    //       },
    //    //       "required": [
    //    //         "vendorId"
    //    //       ]
    //    //     }
    //    //   },
    //    //   "type": "object",
    //    //   "additionalProperties": false,
    //    //   "properties": {
    //    //     "customData": {
    //    //       "$ref": "#/definitions/CustomDataType"
    //    //     },
    //    //     "messageId": {
    //    //       "description": "May be used to indicate a specific message or implementation.\r\n",
    //    //       "type": "string",
    //    //       "maxLength": 50
    //    //     },
    //    //     "data": {
    //    //       "description": "Data without specified length or format. This needs to be decided by both parties (Open to implementation).\r\n"
    //    //     },
    //    //     "vendorId": {
    //    //       "description": "This identifies the Vendor specific implementation\r\n\r\n",
    //    //       "type": "string",
    //    //       "maxLength": 255
    //    //     }
    //    //   },
    //    //   "required": [
    //    //     "vendorId"
    //    //   ]
    //    // }

    //    #endregion

    //    #region (static) Parse   (XML, XMLNamespace, RequestId, Destination, NetworkPath, ...)

    //    /// <summary>
    //    /// Parse the given XML representation of a DataTransfer request.
    //    /// </summary>
    //    /// <param name="XML">The XML to be parsed.</param>
    //    /// <param name="XMLNamespace">The XML namespace to use.</param>
    //    /// <param name="RequestId">The request identification.</param>
    //    /// <param name="Destination">The destination networking node identification or source routing path.</param>
    //    /// <param name="NetworkPath">The network path of the request.</param>
    //    /// <param name="RequestTimestamp">An optional request timestamp.</param>
    //    /// <param name="RequestTimeout">An optional request timeout.</param>
    //    /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
    //    public static DataTransferRequest Parse(XElement           XML,
    //                                            XNamespace         XMLNamespace,
    //                                            Request_Id         RequestId,
    //                                            SourceRouting      Destination,
    //                                            NetworkPath        NetworkPath,
    //                                            DateTime?          RequestTimestamp   = null,
    //                                            TimeSpan?          RequestTimeout     = null,
    //                                            EventTracking_Id?  EventTrackingId    = null)
    //    {

    //        if (TryParse(XML,
    //                     XMLNamespace,
    //                     RequestId,
    //                     Destination,
    //                     NetworkPath,
    //                     out var dataTransferRequest,
    //                     out var errorResponse,
    //                     RequestTimestamp,
    //                     RequestTimeout,
    //                     EventTrackingId))
    //        {
    //            return dataTransferRequest;
    //        }

    //        throw new ArgumentException("The given XML representation of a DataTransfer request is invalid: " + errorResponse,
    //                                    nameof(XML));

    //    }

    //    #endregion

    //    #region (static) Parse   (JSON,              RequestId, Destination, NetworkPath, ...)

    //    /// <summary>
    //    /// Parse the given JSON representation of a DataTransfer request.
    //    /// </summary>
    //    /// <param name="JSON">The JSON to be parsed.</param>
    //    /// <param name="RequestId">The request identification.</param>
    //    /// <param name="Destination">The destination networking node identification or source routing path.</param>
    //    /// <param name="NetworkPath">The network path of the request.</param>
    //    /// <param name="RequestTimestamp">An optional request timestamp.</param>
    //    /// <param name="RequestTimeout">An optional request timeout.</param>
    //    /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
    //    /// <param name="CustomDataTransferRequestParser">A delegate to parse custom DataTransfer requests.</param>
    //    /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
    //    /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
    //    public static DataTransferRequest Parse(JObject                                            JSON,
    //                                            Request_Id                                         RequestId,
    //                                            SourceRouting                                      Destination,
    //                                            NetworkPath                                        NetworkPath,
    //                                            DateTime?                                          RequestTimestamp                  = null,
    //                                            TimeSpan?                                          RequestTimeout                    = null,
    //                                            EventTracking_Id?                                  EventTrackingId                   = null,
    //                                            CustomJObjectParserDelegate<DataTransferRequest>?  CustomDataTransferRequestParser   = null,
    //                                            CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
    //                                            CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
    //    {

    //        if (TryParse(JSON,
    //                     RequestId,
    //                     Destination,
    //                     NetworkPath,
    //                     out var dataTransferRequest,
    //                     out var errorResponse,
    //                     RequestTimestamp,
    //                     RequestTimeout,
    //                     EventTrackingId,
    //                     CustomDataTransferRequestParser,
    //                     CustomSignatureParser,
    //                     CustomCustomDataParser))
    //        {
    //            return dataTransferRequest;
    //        }

    //        throw new ArgumentException("The given JSON representation of a DataTransfer request is invalid: " + errorResponse,
    //                                    nameof(JSON));

    //    }

    //    #endregion

    //    #region (static) TryParse(XML, XMLNamespace, RequestId, Destination, NetworkPath, out DataTransferRequest, out ErrorResponse, ...)

    //    /// <summary>
    //    /// Try to parse the given XML representation of a DataTransfer request.
    //    /// </summary>
    //    /// <param name="XML">The XML to be parsed.</param>
    //    /// <param name="XMLNamespace">The XML namespace to use.</param>
    //    /// <param name="RequestId">The request identification.</param>
    //    /// <param name="Destination">The destination networking node identification or source routing path.</param>
    //    /// <param name="NetworkPath">The network path of the request.</param>
    //    /// <param name="DataTransferRequest">The parsed BootNotification request.</param>
    //    /// <param name="ErrorResponse">An optional error response.</param>
    //    /// <param name="RequestTimestamp">An optional request timestamp.</param>
    //    /// <param name="RequestTimeout">An optional request timeout.</param>
    //    /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
    //    public static Boolean TryParse(XElement                                       XML,
    //                                   XNamespace                                     XMLNamespace,
    //                                   Request_Id                                     RequestId,
    //                                   SourceRouting                                  Destination,
    //                                   NetworkPath                                    NetworkPath,
    //                                   [NotNullWhen(true)]  out DataTransferRequest?  DataTransferRequest,
    //                                   [NotNullWhen(false)] out String?               ErrorResponse,
    //                                   DateTime?                                      RequestTimestamp   = null,
    //                                   TimeSpan?                                      RequestTimeout     = null,
    //                                   EventTracking_Id?                              EventTrackingId    = null)
    //    {

    //        try
    //        {

    //            DataTransferRequest = new DataTransferRequest(
    //                                      Destination,
    //                                      Vendor_Id. Parse(XML.ElementValueOrFail   (XMLNamespace + "vendorId")),
    //                                      Message_Id.Parse(XML.ElementValueOrDefault(XMLNamespace + "messageId")),
    //                                      XML.ElementValueOrDefault(XMLNamespace + "data"),
    //                                      RequestId:    RequestId,
    //                                      NetworkPath:  NetworkPath
    //                                  );

    //            ErrorResponse = null;
    //            return true;

    //        }
    //        catch (Exception e)
    //        {
    //            DataTransferRequest  = null;
    //            ErrorResponse        = "The given XML representation of a DataTransfer request is invalid: " + e.Message;
    //            return false;
    //        }

    //    }

    //    #endregion

    //    #region (static) TryParse(JSON,              RequestId, Destination, NetworkPath, out DataTransferRequest, out ErrorResponse, ...)

    //    /// <summary>
    //    /// Try to parse the given JSON representation of a DataTransfer request.
    //    /// </summary>
    //    /// <param name="JSON">The JSON to be parsed.</param>
    //    /// <param name="RequestId">The request identification.</param>
    //    /// <param name="Destination">The destination networking node identification or source routing path.</param>
    //    /// <param name="NetworkPath">The network path of the request.</param>
    //    /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
    //    /// <param name="ErrorResponse">An optional error response.</param>
    //    /// <param name="RequestTimestamp">An optional request timestamp.</param>
    //    /// <param name="RequestTimeout">An optional request timeout.</param>
    //    /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
    //    /// <param name="CustomDataTransferRequestParser">A delegate to parse custom DataTransfer requests.</param>
    //    /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
    //    /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
    //    public static Boolean TryParse(JObject                                            JSON,
    //                                   Request_Id                                         RequestId,
    //                                   SourceRouting                                      Destination,
    //                                   NetworkPath                                        NetworkPath,
    //                                   [NotNullWhen(true)]  out DataTransferRequest?      DataTransferRequest,
    //                                   [NotNullWhen(false)] out String?                   ErrorResponse,
    //                                   DateTime?                                          RequestTimestamp                  = null,
    //                                   TimeSpan?                                          RequestTimeout                    = null,
    //                                   EventTracking_Id?                                  EventTrackingId                   = null,
    //                                   CustomJObjectParserDelegate<DataTransferRequest>?  CustomDataTransferRequestParser   = null,
    //                                   CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
    //                                   CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
    //    {

    //        try
    //        {

    //            DataTransferRequest = null;

    //            #region VendorId      [mandatory]

    //            if (!JSON.ParseMandatory("vendorId",
    //                                     "vendor identification",
    //                                     Vendor_Id.TryParse,
    //                                     out Vendor_Id VendorId,
    //                                     out ErrorResponse))
    //            {
    //                return false;
    //            }

    //            #endregion

    //            #region MessageId     [optional]

    //            if (JSON.ParseOptional("messageId",
    //                                   "message identification",
    //                                   Message_Id.TryParse,
    //                                   out Message_Id? MessageId,
    //                                   out ErrorResponse))
    //            {
    //                if (ErrorResponse is not null)
    //                    return false;
    //            }

    //            #endregion

    //            #region Data          [optional]

    //            var Data = JSON["data"];

    //            #endregion

    //            #region Signatures    [optional, OCPP_CSE]

    //            if (JSON.ParseOptionalHashSet("signatures",
    //                                          "cryptographic signatures",
    //                                          Signature.TryParse,
    //                                          out HashSet<Signature> Signatures,
    //                                          out ErrorResponse))
    //            {
    //                if (ErrorResponse is not null)
    //                    return false;
    //            }

    //            #endregion

    //            #region CustomData    [optional]

    //            if (JSON.ParseOptionalJSON("customData",
    //                                       "custom data",
    //                                       WWCP.CustomData.TryParse,
    //                                       out CustomData? CustomData,
    //                                       out ErrorResponse))
    //            {
    //                if (ErrorResponse is not null)
    //                    return false;
    //            }

    //            #endregion


    //            DataTransferRequest = new DataTransferRequest(

    //                                      Destination,
    //                                      VendorId,
    //                                      MessageId,
    //                                      Data,

    //                                      null,
    //                                      null,
    //                                      Signatures,

    //                                      RequestId,
    //                                      RequestTimestamp,
    //                                      RequestTimeout,
    //                                      EventTrackingId,
    //                                      NetworkPath

    //                                  );

    //            if (CustomDataTransferRequestParser is not null)
    //                DataTransferRequest = CustomDataTransferRequestParser(JSON,
    //                                                                      DataTransferRequest);

    //            return true;

    //        }
    //        catch (Exception e)
    //        {
    //            DataTransferRequest  = null;
    //            ErrorResponse        = "The given JSON representation of a DataTransfer request is invalid: " + e.Message;
    //            return false;
    //        }

    //    }

    //    #endregion

    //    #region ToXML (XMLNamespace)

    //    /// <summary>
    //    /// Return a XML representation of this object.
    //    /// </summary>
    //    /// <param name="XMLNamespace">The XML namespace to use.</param>
    //    public XElement ToXML(XNamespace XMLNamespace) // OCPPNS.OCPPv1_6_CS

    //        => new (XMLNamespace + "dataTransferRequest",

    //                     new XElement(XMLNamespace + "vendorId",    VendorId),

    //               MessageId.IsNotNullOrEmpty()
    //                   ? new XElement(XMLNamespace + "messageId",   MessageId)
    //                   : null,

    //               Data is not null && Data.Type == JTokenType.String
    //                   ? new XElement(XMLNamespace + "data",        Data.Value<String>())
    //                   : null

    //           );

    //    #endregion

    //    #region ToJSON(CustomDataTransferRequestSerializer = null, CustomSignatureSerializer = null, ...)

    //    /// <summary>
    //    /// Return a JSON representation of this object.
    //    /// </summary>
    //    /// <param name="CustomDataTransferRequestSerializer">A delegate to serialize custom DataTransfer requests.</param>
    //    /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
    //    /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
    //    public JObject ToJSON(JToken?                                                          Data,
    //                          CustomJObjectSerializerDelegate<DataTransferRequest<TRequest>>?  CustomDataTransferRequestSerializer   = null,
    //                          CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer             = null,
    //                          CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer            = null)
    //    {

    //        var json = JSONObject.Create(

    //                             new JProperty("vendorId",     VendorId.       TextId),

    //                       MessageId.HasValue
    //                           ? new JProperty("messageId",    MessageId.Value.TextId)
    //                           : null,

    //                       Data is not null
    //                           ? new JProperty("data",         Data)
    //                           : null,

    //                       Signatures.Any()
    //                           ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
    //                                                                                                                      CustomCustomDataSerializer))))
    //                           : null,

    //                       CustomData is not null
    //                           ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
    //                           : null

    //                   );

    //        return CustomDataTransferRequestSerializer is not null
    //                   ? CustomDataTransferRequestSerializer(this, json)
    //                   : json;

    //    }

    //    #endregion


    //    #region Operator overloading

    //    #region Operator == (DataTransferRequest1, DataTransferRequest2)

    //    /// <summary>
    //    /// Compares two DataTransfer requests for equality.
    //    /// </summary>
    //    /// <param name="DataTransferRequest1">A DataTransfer request.</param>
    //    /// <param name="DataTransferRequest2">Another DataTransfer request.</param>
    //    /// <returns>True if both match; False otherwise.</returns>
    //    public static Boolean operator == (DataTransferRequest<TRequest>? DataTransferRequest1,
    //                                       DataTransferRequest<TRequest>? DataTransferRequest2)
    //    {

    //        // If both are null, or both are same instance, return true.
    //        if (ReferenceEquals(DataTransferRequest1, DataTransferRequest2))
    //            return true;

    //        // If one is null, but not both, return false.
    //        if (DataTransferRequest1 is null || DataTransferRequest2 is null)
    //            return false;

    //        return DataTransferRequest1.Equals(DataTransferRequest2);

    //    }

    //    #endregion

    //    #region Operator != (DataTransferRequest1, DataTransferRequest2)

    //    /// <summary>
    //    /// Compares two DataTransfer requests for inequality.
    //    /// </summary>
    //    /// <param name="DataTransferRequest1">A DataTransfer request.</param>
    //    /// <param name="DataTransferRequest2">Another DataTransfer request.</param>
    //    /// <returns>False if both match; True otherwise.</returns>
    //    public static Boolean operator != (DataTransferRequest<TRequest>? DataTransferRequest1,
    //                                       DataTransferRequest<TRequest>? DataTransferRequest2)

    //        => !(DataTransferRequest1 == DataTransferRequest2);

    //    #endregion

    //    #endregion

    //    #region IEquatable<DataTransferRequest> Members

    //    #region Equals(Object)

    //    /// <summary>
    //    /// Compares two DataTransfer requests for equality.
    //    /// </summary>
    //    /// <param name="Object">A DataTransfer request to compare with.</param>
    //    public override Boolean Equals(Object? Object)

    //        => Object is DataTransferRequest dataTransferRequest &&
    //               Equals(dataTransferRequest);

    //    #endregion

    //    #region Equals(DataTransferRequest)

    //    /// <summary>
    //    /// Compares two DataTransfer requests for equality.
    //    /// </summary>
    //    /// <param name="DataTransferRequest">A DataTransfer request to compare with.</param>
    //    public override Boolean Equals(DataTransferRequest<TRequest>? DataTransferRequest)

    //        => DataTransferRequest is not null               &&

    //           VendorId.Equals(DataTransferRequest.VendorId) &&

    //         ((MessageId is     null && DataTransferRequest.MessageId is     null) ||
    //          (MessageId is not null && DataTransferRequest.MessageId is not null && MessageId.Equals(DataTransferRequest.MessageId))) &&

    //         ((Data      is     null && DataTransferRequest.Data      is     null) ||
    //          (Data      is not null && DataTransferRequest.Data      is not null && Data.     Equals(DataTransferRequest.Data)))      &&

    //           base.GenericEquals(DataTransferRequest);

    //    #endregion

    //    #endregion

    //    #region (override) GetHashCode()

    //    private readonly Int32 hashCode;

    //    /// <summary>
    //    /// Return the hash code of this object.
    //    /// </summary>
    //    public override Int32 GetHashCode()
    //        => hashCode;

    //    #endregion

    //    #region (override) ToString()

    //    /// <summary>
    //    /// Return a text representation of this object.
    //    /// </summary>
    //    public override String ToString()

    //        => String.Concat(

    //               $"'{VendorId}'",

    //               MessageId.IsNotNullOrEmpty()
    //                   ? $" / '{MessageId}'"
    //                   : "",

    //               Data is not null
    //                   ? $" => '{Data}'"
    //                   : ""

    //           );

    //    #endregion


    //}

}
