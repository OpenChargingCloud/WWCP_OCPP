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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The unpublish firmware request.
    /// </summary>
    public class UnpublishFirmwareRequest : ARequest<UnpublishFirmwareRequest>,
                                            IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/unpublishFirmwareRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The MD5 checksum over the entire firmware image as a hexadecimal string of length 32.
        /// </summary>
        [Mandatory]
        public String         MD5Checksum    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new unpublish firmware request.
        /// </summary>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="MD5Checksum">The MD5 checksum over the entire firmware image as a hexadecimal string of length 32.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public UnpublishFirmwareRequest(ChargingStation_Id       ChargingStationId,
                                        String                   MD5Checksum,

                                        IEnumerable<KeyPair>?    SignKeys            = null,
                                        IEnumerable<SignInfo>?   SignInfos           = null,
                                        SignaturePolicy?         SignaturePolicy     = null,
                                        IEnumerable<Signature>?  Signatures          = null,

                                        CustomData?              CustomData          = null,

                                        Request_Id?              RequestId           = null,
                                        DateTime?                RequestTimestamp    = null,
                                        TimeSpan?                RequestTimeout      = null,
                                        EventTracking_Id?        EventTrackingId     = null,
                                        CancellationToken        CancellationToken   = default)

            : base(ChargingStationId,
                   "UnpublishFirmware",

                   SignKeys,
                   SignInfos,
                   SignaturePolicy,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.MD5Checksum = MD5Checksum;

            unchecked
            {

                hashCode = this.MD5Checksum.GetHashCode() * 3 ^
                           base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:UnpublishFirmwareRequest",
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
        //     "checksum": {
        //       "description": "The MD5 checksum over the entire firmware file as a hexadecimal string of length 32. \r\n",
        //       "type": "string",
        //       "maxLength": 32
        //     }
        //   },
        //   "required": [
        //     "checksum"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargingStationId, CustomUnpublishFirmwareRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an unpublish firmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="CustomUnpublishFirmwareRequestParser">A delegate to parse custom unpublish firmware requests.</param>
        public static UnpublishFirmwareRequest Parse(JObject                                                 JSON,
                                                     Request_Id                                              RequestId,
                                                     ChargingStation_Id                                      ChargingStationId,
                                                     CustomJObjectParserDelegate<UnpublishFirmwareRequest>?  CustomUnpublishFirmwareRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargingStationId,
                         out var unpublishFirmwareRequest,
                         out var errorResponse,
                         CustomUnpublishFirmwareRequestParser))
            {
                return unpublishFirmwareRequest!;
            }

            throw new ArgumentException("The given JSON representation of an unpublish firmware request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargingStationId, out UnpublishFirmwareRequest, out ErrorResponse, CustomUnpublishFirmwareRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an unpublish firmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="UnpublishFirmwareRequest">The parsed unpublish firmware request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                        JSON,
                                       Request_Id                     RequestId,
                                       ChargingStation_Id             ChargingStationId,
                                       out UnpublishFirmwareRequest?  UnpublishFirmwareRequest,
                                       out String?                    ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargingStationId,
                        out UnpublishFirmwareRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an unpublish firmware request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="UnpublishFirmwareRequest">The parsed unpublish firmware request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnpublishFirmwareRequestParser">A delegate to parse custom unpublish firmware requests.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       ChargingStation_Id                                      ChargingStationId,
                                       out UnpublishFirmwareRequest?                           UnpublishFirmwareRequest,
                                       out String?                                             ErrorResponse,
                                       CustomJObjectParserDelegate<UnpublishFirmwareRequest>?  CustomUnpublishFirmwareRequestParser)
        {

            try
            {

                UnpublishFirmwareRequest = null;

                #region MD5Checksum          [mandatory]

                if (!JSON.ParseMandatoryText("checksum",
                                             "MD5 checksum",
                                             out String MD5Checksum,
                                             out ErrorResponse))
                {
                    return false;
                }

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

                #region ChargingStationId    [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargingStationId",
                                       "charging station identification",
                                       ChargingStation_Id.TryParse,
                                       out ChargingStation_Id? chargingStationId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargingStationId_PayLoad.HasValue)
                        ChargingStationId = chargingStationId_PayLoad.Value;

                }

                #endregion


                UnpublishFirmwareRequest = new UnpublishFirmwareRequest(
                                               ChargingStationId,
                                               MD5Checksum,
                                               null,
                                               null,
                                               null,
                                               Signatures,
                                               CustomData,
                                               RequestId
                                           );

                if (CustomUnpublishFirmwareRequestParser is not null)
                    UnpublishFirmwareRequest = CustomUnpublishFirmwareRequestParser(JSON,
                                                                                    UnpublishFirmwareRequest);

                return true;

            }
            catch (Exception e)
            {
                UnpublishFirmwareRequest  = null;
                ErrorResponse             = "The given JSON representation of an unpublish firmware request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUnpublishFirmwareRequestSerializer = null, CustomFirmwareSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnpublishFirmwareRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnpublishFirmwareRequest>?  CustomUnpublishFirmwareRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("checksum",     MD5Checksum),


                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUnpublishFirmwareRequestSerializer is not null
                       ? CustomUnpublishFirmwareRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UnpublishFirmwareRequest1, UnpublishFirmwareRequest2)

        /// <summary>
        /// Compares two unpublish firmware requests for equality.
        /// </summary>
        /// <param name="UnpublishFirmwareRequest1">An unpublish firmware request.</param>
        /// <param name="UnpublishFirmwareRequest2">Another unpublish firmware request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnpublishFirmwareRequest? UnpublishFirmwareRequest1,
                                           UnpublishFirmwareRequest? UnpublishFirmwareRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnpublishFirmwareRequest1, UnpublishFirmwareRequest2))
                return true;

            // If one is null, but not both, return false.
            if (UnpublishFirmwareRequest1 is null || UnpublishFirmwareRequest2 is null)
                return false;

            return UnpublishFirmwareRequest1.Equals(UnpublishFirmwareRequest2);

        }

        #endregion

        #region Operator != (UnpublishFirmwareRequest1, UnpublishFirmwareRequest2)

        /// <summary>
        /// Compares two unpublish firmware requests for inequality.
        /// </summary>
        /// <param name="UnpublishFirmwareRequest1">An unpublish firmware request.</param>
        /// <param name="UnpublishFirmwareRequest2">Another unpublish firmware request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnpublishFirmwareRequest? UnpublishFirmwareRequest1,
                                           UnpublishFirmwareRequest? UnpublishFirmwareRequest2)

            => !(UnpublishFirmwareRequest1 == UnpublishFirmwareRequest2);

        #endregion

        #endregion

        #region IEquatable<UnpublishFirmwareRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two unpublish firmware requests for equality.
        /// </summary>
        /// <param name="Object">An unpublish firmware request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnpublishFirmwareRequest unpublishFirmwareRequest &&
                   Equals(unpublishFirmwareRequest);

        #endregion

        #region Equals(UnpublishFirmwareRequest)

        /// <summary>
        /// Compares two unpublish firmware requests for equality.
        /// </summary>
        /// <param name="UnpublishFirmwareRequest">An unpublish firmware request to compare with.</param>
        public override Boolean Equals(UnpublishFirmwareRequest? UnpublishFirmwareRequest)

            => UnpublishFirmwareRequest is not null &&

               MD5Checksum.Equals(UnpublishFirmwareRequest.MD5Checksum) &&

               base.GenericEquals(UnpublishFirmwareRequest);

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

            => MD5Checksum;

        #endregion

    }

}
