/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The data transfer request.
    /// </summary>
    public class DataTransferRequest : ARequest<DataTransferRequest>,
                                       IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/dataTransferRequest");

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
        /// The optional message identification.
        /// </summary>
        [Optional]
        public Message_Id?    MessageId    { get; }

        /// <summary>
        /// Optional vendor-specific message data (a JSON token).
        /// </summary>
        [Optional]
        public JToken?        Data         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new data transfer request.
        /// </summary>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification.</param>
        /// <param name="Data">Optional vendor-specific message data (a JSON token).</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public DataTransferRequest(NetworkingNode_Id        NetworkingNodeId,
                                   Vendor_Id                VendorId,
                                   Message_Id?              MessageId           = null,
                                   JToken?                  Data                = null,

                                   IEnumerable<KeyPair>?    SignKeys            = null,
                                   IEnumerable<SignInfo>?   SignInfos           = null,
                                   IEnumerable<Signature>?  Signatures          = null,

                                   CustomData?              CustomData          = null,

                                   Request_Id?              RequestId           = null,
                                   DateTime?                RequestTimestamp    = null,
                                   TimeSpan?                RequestTimeout      = null,
                                   EventTracking_Id?        EventTrackingId     = null,
                                   NetworkPath?             NetworkPath         = null,
                                   CancellationToken        CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(DataTransferRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
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

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomDataTransferRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a data transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomDataTransferRequestParser">A delegate to parse custom data transfer requests.</param>
        public static DataTransferRequest Parse(JObject                                            JSON,
                                                Request_Id                                         RequestId,
                                                NetworkingNode_Id                                  NetworkingNodeId,
                                                NetworkPath                                        NetworkPath,
                                                CustomJObjectParserDelegate<DataTransferRequest>?  CustomDataTransferRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var dataTransferRequest,
                         out var errorResponse,
                         CustomDataTransferRequestParser) &&
                dataTransferRequest is not null)
            {
                return dataTransferRequest;
            }

            throw new ArgumentException("The given JSON representation of a data transfer request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out DataTransferRequest, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a data transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                   JSON,
                                       Request_Id                RequestId,
                                       NetworkingNode_Id         NetworkingNodeId,
                                       NetworkPath               NetworkPath,
                                       out DataTransferRequest?  DataTransferRequest,
                                       out String?               ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out DataTransferRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a data transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The sending charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="DataTransferRequest">The parsed DataTransfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDataTransferRequestParser">A delegate to parse custom DataTransfer requests.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       Request_Id                                         RequestId,
                                       NetworkingNode_Id                                  NetworkingNodeId,
                                       NetworkPath                                        NetworkPath,
                                       out DataTransferRequest?                           DataTransferRequest,
                                       out String?                                        ErrorResponse,
                                       CustomJObjectParserDelegate<DataTransferRequest>?  CustomDataTransferRequestParser)
        {

            try
            {

                DataTransferRequest = null;

                #region VendorId             [mandatory]

                if (!JSON.ParseMandatory("vendorId",
                                         "vendor identification",
                                         Vendor_Id.TryParse,
                                         out Vendor_Id VendorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MessageId            [optional]

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

                #region Data                 [optional]

                var Data = JSON["data"];

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                DataTransferRequest = new DataTransferRequest(

                                          NetworkingNodeId,
                                          VendorId,
                                          MessageId,
                                          Data,

                                          null,
                                          null,
                                          Signatures,

                                          CustomData,

                                          RequestId,
                                          null,
                                          null,
                                          null,
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
                ErrorResponse        = "The given JSON representation of a data transfer request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDataTransferRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDataTransferRequestSerializer">A delegate to serialize custom data transfer requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DataTransferRequest>?  CustomDataTransferRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("vendorId",     VendorId.       InternalId),

                           MessageId.HasValue
                               ? new JProperty("messageId",    MessageId.Value.InternalId)
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

            => $"{VendorId}: {MessageId?.ToString() ?? "-"} => {Data?.ToString() ?? "-"}";

        #endregion


    }

}
