/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP <https://github.com/OpenChargingCloud/WWCP_OCPP>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The DataTransfer response.
    /// </summary>
    public class DataTransferResponse : AResponse<DataTransferRequest,
                                                  DataTransferResponse>,
                                        IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/dataTransferResponse");

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

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?         StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DataTransfer response.
        /// </summary>
        /// <param name="Request">The DataTransfer request leading to this response.</param>
        /// <param name="Status">The success or failure status of the DataTransfer.</param>
        /// <param name="Data">A vendor-specific JSON token.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
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
                                    StatusInfo?              StatusInfo            = null,

                                    Result?                  Result                = null,
                                    DateTime?                ResponseTimestamp     = null,

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

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.Status      = Status;
            this.Data        = Data;
            this.StatusInfo  = StatusInfo;

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:DataTransferResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "DataTransferStatusEnumType": {
        //             "description": "This indicates the success or failure of the data transfer.\r\n",
        //             "javaType": "DataTransferStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
        //                 "UnknownMessageId",
        //                 "UnknownVendorId"
        //             ]
        //         },
        //         "StatusInfoType": {
        //             "description": "Element providing more information about the status.\r\n",
        //             "javaType": "StatusInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "reasonCode": {
        //                     "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.\r\n",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "additionalInfo": {
        //                     "description": "Additional text to provide detailed information.\r\n",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "reasonCode"
        //             ]
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "status": {
        //             "$ref": "#/definitions/DataTransferStatusEnumType"
        //         },
        //         "statusInfo": {
        //             "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "data": {
        //             "description": "Data without specified length or format, in response to request.\r\n"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a DataTransfer response.
        /// </summary>
        /// <param name="Request">The DataTransfer request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDataTransferResponseParser">An optional delegate to parse custom DataTransfer responses.</param>
        public static DataTransferResponse Parse(DataTransferRequest                                 Request,
                                                 JObject                                             JSON,
                                                 SourceRouting                                       Destination,
                                                 NetworkPath                                         NetworkPath,
                                                 DateTime?                                           ResponseTimestamp                 = null,
                                                 CustomJObjectParserDelegate<DataTransferResponse>?  CustomDataTransferResponseParser  = null,
                                                 CustomJObjectParserDelegate<StatusInfo>?            CustomStatusInfoParser            = null,
                                                 CustomJObjectParserDelegate<Signature>?             CustomSignatureParser             = null,
                                                 CustomJObjectParserDelegate<CustomData>?            CustomCustomDataParser            = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var dataTransferResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomDataTransferResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return dataTransferResponse;
            }

            throw new ArgumentException("The given JSON representation of a DataTransfer response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out DataTransferResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a DataTransfer response.
        /// </summary>
        /// <param name="Request">The DataTransfer request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="DataTransferResponse">The parsed DataTransfer response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomDataTransferResponseParser">An optional delegate to parse custom DataTransfer responses.</param>
        public static Boolean TryParse(DataTransferRequest                                 Request,
                                       JObject                                             JSON,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out DataTransferResponse?      DataTransferResponse,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       DateTime?                                           ResponseTimestamp                  = null,
                                       CustomJObjectParserDelegate<DataTransferResponse>?  CustomDataTransferResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?            CustomStatusInfoParser             = null,
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

                #region StatusInfo            [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           (JObject json, [NotNullWhen(true)] out StatusInfo? statusInfo, [NotNullWhen(false)] out String? errorResponse)
                                                 => OCPPv2_1.StatusInfo.TryParse(json, out statusInfo, out errorResponse, CustomStatusInfoParser),
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
                                              (JObject json, [NotNullWhen(true)] out Signature? signature, [NotNullWhen(false)] out String? errorResponse)
                                                  => Signature.TryParse(json, out signature, out errorResponse, CustomSignatureParser, CustomCustomDataParser),
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
                                           (JObject json, [NotNullWhen(true)] out CustomData? customData, [NotNullWhen(false)] out String? errorResponse)
                                                => WWCP.CustomData.TryParse(json, out customData, out errorResponse, CustomCustomDataParser),
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
                                           StatusInfo,

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

        #region ToJSON(CustomDataTransferResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDataTransferResponseSerializer">A delegate to serialize custom DataTransfer responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                 IncludeJSONLDContext                   = false,
                              CustomJObjectSerializerDelegate<DataTransferResponse>?  CustomDataTransferResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?            CustomStatusInfoSerializer             = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",       Status.              AsText()),

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
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
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
        /// <param name="Request">The DataTransfer request.</param>
        public static DataTransferResponse RequestError(DataTransferRequest      Request,
                                                        EventTracking_Id         EventTrackingId,
                                                        ResultCode               ErrorCode,
                                                        String?                  ErrorDescription    = null,
                                                        JObject?                 ErrorDetails        = null,
                                                        DateTime?                ResponseTimestamp   = null,

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
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The DataTransfer failed.
        /// </summary>
        /// <param name="Request">The DataTransfer request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static DataTransferResponse SignatureError(DataTransferRequest  Request,
                                                          String               ErrorDescription)

            => new (Request,
                    DataTransferStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The DataTransfer failed.
        /// </summary>
        /// <param name="Request">The DataTransfer request.</param>
        /// <param name="Description">An optional error description.</param>
        public static DataTransferResponse Failed(DataTransferRequest  Request,
                                                  String?              Description   = null)

            => new (Request,
                    DataTransferStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The DataTransfer failed because of an exception.
        /// </summary>
        /// <param name="Request">The DataTransfer request.</param>
        /// <param name="Exception">The exception.</param>
        public static DataTransferResponse ExceptionOccured(DataTransferRequest  Request,
                                                            Exception            Exception)

            => new (Request,
                    DataTransferStatus.Rejected,
                    Result:  Result.FromException(Exception));

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
                       ? $", {Data}"
                       : ""

               );

        #endregion

    }

}
