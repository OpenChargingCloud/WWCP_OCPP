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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A DataTransfer response.
    /// </summary>
    public class DataTransferResponse : AResponse<DataTransferRequest,
                                                  DataTransferResponse>,
                                        IResponse<Result>
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
        /// The success or failure status of the DataTransfer.
        /// </summary>
        [Mandatory]
        public DataTransferStatus  Status        { get; }

        /// <summary>
        /// Optional response data.
        /// </summary>
        [Optional]
        public JToken?             Data          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DataTransfer response.
        /// </summary>
        /// <param name="Request">The DataTransfer request leading to this response.</param>
        /// <param name="Status">The success or failure status of the DataTransfer.</param>
        /// <param name="Data">A vendor-specific JSON token.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DataTransferResponse(DataTransferRequest      Request,
                                    DataTransferStatus       Status,
                                    JToken?                  Data                  = null,

                                    Result?                  Result                = null,
                                    DateTimeOffset?          ResponseTimestamp     = null,

                                    SourceRouting?           Destination           = null,
                                    NetworkPath?             NetworkPath           = null,

                                    IEnumerable<KeyPair>?    SignKeys              = null,
                                    IEnumerable<SignInfo>?   SignInfos             = null,
                                    IEnumerable<Signature>?  Signatures            = null,

                                    CustomData?              CustomData            = null,

                                    SerializationFormats?    SerializationFormat   = null,
                                    CancellationToken        CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat,
                   CancellationToken)

        {

            this.Status  = Status;
            this.Data    = Data;

            unchecked
            {

                hashCode = this.Status.GetHashCode()       * 5 ^
                          (this.Data?. GetHashCode() ?? 0) * 3 ^
                           base.       GetHashCode();

            }

        }

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

        #endregion

        #region (static) Parse   (Request, XML, XMLNamespace, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a DataTransfer response.
        /// </summary>
        /// <param name="Request">The incoming DataTransfer request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="XMLNamespace">A XML namespace.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static DataTransferResponse Parse(DataTransferRequest  Request,
                                                 XElement             XML,
                                                 XNamespace           XMLNamespace,
                                                 SourceRouting        Destination,
                                                 NetworkPath          NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         XMLNamespace,
                         Destination,
                         NetworkPath,
                         out var dataTransferResponse,
                         out var errorResponse))
            {
                return dataTransferResponse;
            }

            throw new ArgumentException("The given XML representation of a DataTransfer response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON,              Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a DataTransfer response.
        /// </summary>
        /// <param name="Request">The DataTransfer request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomDataTransferResponseParser">An optional delegate to parse custom DataTransfer responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static DataTransferResponse Parse(DataTransferRequest                                 Request,
                                                 JObject                                             JSON,
                                                 SourceRouting                                       Destination,
                                                 NetworkPath                                         NetworkPath,
                                                 DateTimeOffset?                                     ResponseTimestamp                  = null,
                                                 CustomJObjectParserDelegate<DataTransferResponse>?  CustomDataTransferResponseParser   = null,
                                                 CustomJObjectParserDelegate<Signature>?             CustomSignatureParser              = null,
                                                 CustomJObjectParserDelegate<CustomData>?            CustomCustomDataParser             = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var dataTransferResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomDataTransferResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return dataTransferResponse;
            }

            throw new ArgumentException("The given JSON representation of a DataTransfer response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML, XMLNamespace, out DataTransferResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a DataTransfer response.
        /// </summary
        /// <param name="Request">The incoming DataTransfer request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="XMLNamespace">A XML namespace.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="DataTransferResponse">The parsed DataTransfer response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(DataTransferRequest                             Request,
                                       XElement                                        XML,
                                       XNamespace                                      XMLNamespace,
                                       SourceRouting                                   Destination,
                                       NetworkPath                                     NetworkPath,
                                       [NotNullWhen(true)]  out DataTransferResponse?  DataTransferResponse,
                                       [NotNullWhen(false)] out String?                ErrorResponse)
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
                ErrorResponse         = "The given XML representation of a DataTransfer response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON,              out DataTransferResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a DataTransfer response.
        /// </summary>
        /// <param name="Request">The DataTransfer request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DataTransferResponse">The parsed DataTransfer response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDataTransferResponseParser">An optional delegate to parse custom DataTransfer responses.</param>
        public static Boolean TryParse(DataTransferRequest                                 Request,
                                       JObject                                             JSON,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out DataTransferResponse?      DataTransferResponse,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       DateTimeOffset?                                     ResponseTimestamp                  = null,
                                       CustomJObjectParserDelegate<DataTransferResponse>?  CustomDataTransferResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?             CustomSignatureParser              = null,
                                       CustomJObjectParserDelegate<CustomData>?            CustomCustomDataParser             = null)
        {

            try
            {

                DataTransferResponse = null;

                #region DataTransferStatus    [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "DataTransfer status",
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

                #region Signatures            [optional, OCPP_CSE]

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

                #region CustomData            [optional]

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


                DataTransferResponse = new DataTransferResponse(

                                           Request,
                                           DataTransferStatus,
                                           Data,

                                           null,
                                           ResponseTimestamp,

                                           Destination,
                                           NetworkPath,

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
                ErrorResponse         = "The given JSON representation of a DataTransfer response is invalid: " + e.Message;
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
        /// <param name="CustomDataTransferResponseSerializer">A delegate to serialize custom DataTransfer responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DataTransferResponse>?  CustomDataTransferResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

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

            return CustomDataTransferResponseSerializer is not null
                       ? CustomDataTransferResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The DataTransfer failed because of a request error.
        /// </summary>
        /// <param name="Request">The DataTransfer request leading to this response.</param>
        public static DataTransferResponse RequestError(DataTransferRequest      Request,
                                                        EventTracking_Id         EventTrackingId,
                                                        ResultCode               ErrorCode,
                                                        String?                  ErrorDescription    = null,
                                                        JObject?                 ErrorDetails        = null,
                                                        DateTimeOffset?          ResponseTimestamp   = null,

                                                        SourceRouting?           Destination         = null,
                                                        NetworkPath?             NetworkPath         = null,

                                                        IEnumerable<KeyPair>?    SignKeys            = null,
                                                        IEnumerable<SignInfo>?   SignInfos           = null,
                                                        IEnumerable<Signature>?  Signatures          = null,

                                                        CustomData?              CustomData          = null)

            => new (

                   Request,
                   DataTransferStatus.Rejected,
                   null,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The DataTransfer failed.
        /// </summary>
        /// <param name="Request">The DataTransfer request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static DataTransferResponse FormationViolation(DataTransferRequest  Request,
                                                              String               ErrorDescription)

            => new (Request,
                    DataTransferStatus.Rejected,
                    null,
                    Result:               Result.FormationViolation(
                                              $"Invalid data format: {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The DataTransfer failed.
        /// </summary>
        /// <param name="Request">The DataTransfer request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static DataTransferResponse SignatureError(DataTransferRequest  Request,
                                                          String               ErrorDescription)

            => new (Request,
                    DataTransferStatus.SignatureError,
                    null,
                    Result:               Result.SignatureError(
                                              $"Invalid signature(s): {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The DataTransfer failed.
        /// </summary>
        /// <param name="Request">The DataTransfer request.</param>
        /// <param name="Description">An optional error description.</param>
        public static DataTransferResponse Failed(DataTransferRequest  Request,
                                                  String?              Description   = null)

            => new (Request,
                    DataTransferStatus.Error,
                    null,
                    Result:               Result.Server(Description),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The DataTransfer failed because of an exception.
        /// </summary>
        /// <param name="Request">The DataTransfer request.</param>
        /// <param name="Exception">The exception.</param>
        public static DataTransferResponse ExceptionOccurred(DataTransferRequest  Request,
                                                            Exception            Exception)

            => new (Request,
                    DataTransferStatus.Error,
                    null,
                    Result:               Result.FromException(Exception),
                    SerializationFormat:  Request.SerializationFormat);

        #endregion


        #region Operator overloading

        #region Operator == (DataTransferResponse1, DataTransferResponse2)

        /// <summary>
        /// Compares two DataTransfer responses for equality.
        /// </summary>
        /// <param name="DataTransferResponse1">A DataTransfer response.</param>
        /// <param name="DataTransferResponse2">Another DataTransfer response.</param>
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
        /// Compares two DataTransfer responses for inequality.
        /// </summary>
        /// <param name="DataTransferResponse1">A DataTransfer response.</param>
        /// <param name="DataTransferResponse2">Another DataTransfer response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DataTransferResponse? DataTransferResponse1,
                                           DataTransferResponse? DataTransferResponse2)

            => !(DataTransferResponse1 == DataTransferResponse2);

        #endregion

        #endregion

        #region IEquatable<DataTransferResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DataTransfer responses for equality.
        /// </summary>
        /// <param name="Object">A DataTransfer response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DataTransferResponse dataTransferResponse &&
                   Equals(dataTransferResponse);

        #endregion

        #region Equals(DataTransferResponse)

        /// <summary>
        /// Compares two DataTransfer responses for equality.
        /// </summary>
        /// <param name="DataTransferResponse">A DataTransfer response to compare with.</param>
        public override Boolean Equals(DataTransferResponse? DataTransferResponse)

            => DataTransferResponse is not null &&

               Status.     Equals(DataTransferResponse.Status) &&

             ((Data       is     null && DataTransferResponse.Data       is     null) ||
              (Data       is not null && DataTransferResponse.Data       is not null && Data.      Equals(DataTransferResponse.Data)))      &&

               base.GenericEquals(DataTransferResponse);

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
                   Status,
                   Data is not null
                       ? ", " + Data
                       : ""
               );

        #endregion

    }

}
