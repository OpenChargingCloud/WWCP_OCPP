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
    public class DataTransferRequest : ARequest<DataTransferRequest>
    {

        #region Properties

        /// <summary>
        /// The vendor identification or namespace of the given message.
        /// </summary>
        [Mandatory]
        public Vendor_Id  VendorId     { get; }

        /// <summary>
        /// The optional message identification.
        /// </summary>
        [Optional]
        public String?    MessageId    { get; }

        /// <summary>
        /// Optional vendor-specific message data (a JSON token).
        /// </summary>
        [Optional]
        public JToken?    Data         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new data transfer request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification.</param>
        /// <param name="Data">Optional vendor-specific message data (a JSON token).</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public DataTransferRequest(ChargeBox_Id       ChargeBoxId,
                                   Vendor_Id          VendorId,
                                   String?            MessageId           = null,
                                   JToken?            Data                = null,
                                   CustomData?        CustomData          = null,

                                   Request_Id?        RequestId           = null,
                                   DateTime?          RequestTimestamp    = null,
                                   TimeSpan?          RequestTimeout      = null,
                                   EventTracking_Id?  EventTrackingId     = null,
                                   CancellationToken  CancellationToken   = default)

            : base(ChargeBoxId,
                   "DataTransfer",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.VendorId   = VendorId;
            this.MessageId  = MessageId?.Trim();
            this.Data       = Data;

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

                if (!JSON.ParseMandatory("vendorId",
                                         "vendor identification",
                                         Vendor_Id.TryParse,
                                         out Vendor_Id VendorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MessageId       [optional]

                var MessageId = JSON.GetString("messageId");

                #endregion

                #region Data            [optional]

                var Data = JSON["data"];

                #endregion

                #region CustomData      [optional]

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


                DataTransferRequest = new DataTransferRequest(
                                          ChargeBoxId,
                                          VendorId,
                                          MessageId,
                                          Data,
                                          CustomData,
                                          RequestId
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

        #region ToJSON(CustomDataTransferRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDataTransferRequestSerializer">A delegate to serialize custom data transfer requests.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DataTransferRequest>?  CustomDataTransferRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("vendorId",     VendorId.  ToString()),

                           MessageId.IsNotNullOrEmpty()
                               ? new JProperty("messageId",    MessageId)
                               : null,

                           Data is not null
                               ? new JProperty("data",         Data)
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

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return VendorId.  GetHashCode()       * 7 ^
                      (MessageId?.GetHashCode() ?? 0) * 5 ^
                      (Data?.     GetHashCode() ?? 0) * 3 ^

                       base.      GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(
                   VendorId,        ", ",
                   MessageId ?? "", ", ",
                   Data      ?? ""
               );

        #endregion

    }

}
