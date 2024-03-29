﻿/*
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.CSMS
{

    /// <summary>
    /// A clear cache request.
    /// </summary>
    public class ClearCacheRequest : ARequest<ClearCacheRequest>
    {

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearCache request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public ClearCacheRequest(ChargeBox_Id       ChargeBoxId,
                                 CustomData?        CustomData          = null,

                                 Request_Id?        RequestId           = null,
                                 DateTime?          RequestTimestamp    = null,
                                 TimeSpan?          RequestTimeout      = null,
                                 EventTracking_Id?  EventTrackingId     = null,
                                 CancellationToken  CancellationToken   = default)

            : base(ChargeBoxId,
                   "ClearCache",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        { }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ClearCacheRequest",
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
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomClearCacheRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a clear cache request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomClearCacheRequestParser">An optional delegate to parse custom ClearCache requests.</param>
        public static ClearCacheRequest Parse(JObject                                          JSON,
                                              Request_Id                                       RequestId,
                                              ChargeBox_Id                                     ChargeBoxId,
                                              CustomJObjectParserDelegate<ClearCacheRequest>?  CustomClearCacheRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var clearCacheRequest,
                         out var errorResponse,
                         CustomClearCacheRequestParser))
            {
                return clearCacheRequest!;
            }

            throw new ArgumentException("The given JSON representation of a clear cache request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out ClearCacheRequest, out ErrorResponse, CustomClearCacheRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a clear cache request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ClearCacheRequest">The parsed ClearCache request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                 JSON,
                                       Request_Id              RequestId,
                                       ChargeBox_Id            ChargeBoxId,
                                       out ClearCacheRequest?  ClearCacheRequest,
                                       out String?             ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out ClearCacheRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a clear cache request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ClearCacheRequest">The parsed ClearCache request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearCacheRequestParser">An optional delegate to parse custom ClearCache requests.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       Request_Id                                       RequestId,
                                       ChargeBox_Id                                     ChargeBoxId,
                                       out ClearCacheRequest?                           ClearCacheRequest,
                                       out String?                                      ErrorResponse,
                                       CustomJObjectParserDelegate<ClearCacheRequest>?  CustomClearCacheRequestParser)
        {

            try
            {

                ClearCacheRequest = default;

                #region CustomData     [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId    [optional, OCPP_CSE]

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


                ClearCacheRequest = new ClearCacheRequest(ChargeBoxId,
                                                          CustomData,
                                                          RequestId);

                if (CustomClearCacheRequestParser is not null)
                    ClearCacheRequest = CustomClearCacheRequestParser(JSON,
                                                                      ClearCacheRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearCacheRequest  = null;
                ErrorResponse      = "The given JSON representation of a clear cache request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearCacheRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearCacheRequestSerializer">A delegate to serialize custom ClearCache requests.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearCacheRequest>?  CustomClearCacheRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                           CustomData is not null
                               ? new JProperty("customData", CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearCacheRequestSerializer is not null
                       ? CustomClearCacheRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearCacheRequest1, ClearCacheRequest2)

        /// <summary>
        /// Compares two ClearCache requests for equality.
        /// </summary>
        /// <param name="ClearCacheRequest1">A ClearCache request.</param>
        /// <param name="ClearCacheRequest2">Another ClearCache request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearCacheRequest? ClearCacheRequest1,
                                           ClearCacheRequest? ClearCacheRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearCacheRequest1, ClearCacheRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ClearCacheRequest1 is null || ClearCacheRequest2 is null)
                return false;

            return ClearCacheRequest1.Equals(ClearCacheRequest2);

        }

        #endregion

        #region Operator != (ClearCacheRequest1, ClearCacheRequest2)

        /// <summary>
        /// Compares two ClearCache requests for inequality.
        /// </summary>
        /// <param name="ClearCacheRequest1">A ClearCache request.</param>
        /// <param name="ClearCacheRequest2">Another ClearCache request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearCacheRequest? ClearCacheRequest1,
                                           ClearCacheRequest? ClearCacheRequest2)

            => !(ClearCacheRequest1 == ClearCacheRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearCacheRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two clear cache requests for equality.
        /// </summary>
        /// <param name="Object">A clear cache request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearCacheRequest clearCacheRequest &&
                   Equals(clearCacheRequest);

        #endregion

        #region Equals(ClearCacheRequest)

        /// <summary>
        /// Compares two clear cache requests for equality.
        /// </summary>
        /// <param name="ClearCacheRequest">A clear cache request to compare with.</param>
        public override Boolean Equals(ClearCacheRequest? ClearCacheRequest)

            => ClearCacheRequest is not null &&

               base.GenericEquals(ClearCacheRequest);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "ClearCacheRequest";

        #endregion

    }

}
