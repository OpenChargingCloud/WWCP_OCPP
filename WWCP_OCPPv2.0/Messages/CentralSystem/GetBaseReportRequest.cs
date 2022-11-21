/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    /// <summary>
    /// The get base report request.
    /// </summary>
    public class GetBaseReportRequest : ARequest<GetBaseReportRequest>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the get base report request.
        /// </summary>
        [Mandatory]
        public Int64        GetBaseReportRequestId    { get; }

        /// <summary>
        /// The requested reporting base.
        /// </summary>
        [Mandatory]
        public ReportBases  ReportBase                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a get base report request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="GetBaseReportRequestId">An unique identification of the get base report request.</param>
        /// <param name="ReportBase">The requested reporting base.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetBaseReportRequest(ChargeBox_Id        ChargeBoxId,

                                    Int64               GetBaseReportRequestId,
                                    ReportBases         ReportBase,

                                    CustomData?         CustomData          = null,
                                    Request_Id?         RequestId           = null,
                                    DateTime?           RequestTimestamp    = null,
                                    TimeSpan?           RequestTimeout      = null,
                                    EventTracking_Id?   EventTrackingId     = null,
                                    CancellationToken?  CancellationToken   = null)

            : base(ChargeBoxId,
                   "GetBaseReport",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.GetBaseReportRequestId  = GetBaseReportRequestId;
            this.ReportBase              = ReportBase;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetBaseReportRequest",
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
        //     "ReportBaseEnumType": {
        //       "description": "This field specifies the report base.\r\n",
        //       "javaType": "ReportBaseEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "ConfigurationInventory",
        //         "FullInventory",
        //         "SummaryInventory"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "requestId": {
        //       "description": "The Id of the request.\r\n",
        //       "type": "integer"
        //     },
        //     "reportBase": {
        //       "$ref": "#/definitions/ReportBaseEnumType"
        //     }
        //   },
        //   "required": [
        //     "requestId",
        //     "reportBase"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomGetBaseReportRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get base report request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomGetBaseReportRequestParser">A delegate to parse custom get base report requests.</param>
        public static GetBaseReportRequest Parse(JObject                                             JSON,
                                                 Request_Id                                          RequestId,
                                                 ChargeBox_Id                                        ChargeBoxId,
                                                 CustomJObjectParserDelegate<GetBaseReportRequest>?  CustomGetBaseReportRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var getBaseReportRequest,
                         out var errorResponse,
                         CustomGetBaseReportRequestParser))
            {
                return getBaseReportRequest!;
            }

            throw new ArgumentException("The given JSON representation of a get base report request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out GetBaseReportRequest, out ErrorResponse, CustomRemoteStartTransactionRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get base report request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetBaseReportRequest">The parsed GetBaseReport request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                    JSON,
                                       Request_Id                 RequestId,
                                       ChargeBox_Id               ChargeBoxId,
                                       out GetBaseReportRequest?  GetBaseReportRequest,
                                       out String?                ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out GetBaseReportRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get base report request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetBaseReportRequest">The parsed get base report request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetBaseReportRequestParser">A delegate to parse custom get base report requests.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       Request_Id                                          RequestId,
                                       ChargeBox_Id                                        ChargeBoxId,
                                       out GetBaseReportRequest?                           GetBaseReportRequest,
                                       out String?                                         ErrorResponse,
                                       CustomJObjectParserDelegate<GetBaseReportRequest>?  CustomGetBaseReportRequestParser)
        {

            try
            {

                GetBaseReportRequest = null;

                #region GetBaseReportRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "certificate chain",
                                         out Int64 GetBaseReportRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ReportBase                [mandatory]

                if (!JSON.ParseMandatory("reportBase",
                                         "report base",
                                         ReportBasesExtentions.TryParse,
                                         out ReportBases ReportBase,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData                [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId               [optional, OCPP_CSE]

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


                GetBaseReportRequest = new GetBaseReportRequest(ChargeBoxId,
                                                                GetBaseReportRequestId,
                                                                ReportBase,
                                                                CustomData,
                                                                RequestId);

                if (CustomGetBaseReportRequestParser is not null)
                    GetBaseReportRequest = CustomGetBaseReportRequestParser(JSON,
                                                                                        GetBaseReportRequest);

                return true;

            }
            catch (Exception e)
            {
                GetBaseReportRequest  = null;
                ErrorResponse               = "The given JSON representation of a get base report request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetBaseReportRequestSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetBaseReportRequestSerializer">A delegate to serialize custom get base report requests.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetBaseReportRequest>?  CustomGetBaseReportRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataResponseSerializer     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestId",   GetBaseReportRequestId.ToString()),
                                 new JProperty("reportBase",  ReportBase.            AsText()),

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.            ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomGetBaseReportRequestSerializer is not null
                       ? CustomGetBaseReportRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetBaseReportRequest1, GetBaseReportRequest2)

        /// <summary>
        /// Compares two get base report requests for equality.
        /// </summary>
        /// <param name="GetBaseReportRequest1">A get base report request.</param>
        /// <param name="GetBaseReportRequest2">Another get base report request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetBaseReportRequest? GetBaseReportRequest1,
                                           GetBaseReportRequest? GetBaseReportRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetBaseReportRequest1, GetBaseReportRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetBaseReportRequest1 is null || GetBaseReportRequest2 is null)
                return false;

            return GetBaseReportRequest1.Equals(GetBaseReportRequest2);

        }

        #endregion

        #region Operator != (GetBaseReportRequest1, GetBaseReportRequest2)

        /// <summary>
        /// Compares two get base report requests for inequality.
        /// </summary>
        /// <param name="GetBaseReportRequest1">A get base report request.</param>
        /// <param name="GetBaseReportRequest2">Another get base report request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetBaseReportRequest? GetBaseReportRequest1,
                                           GetBaseReportRequest? GetBaseReportRequest2)

            => !(GetBaseReportRequest1 == GetBaseReportRequest2);

        #endregion

        #endregion

        #region IEquatable<GetBaseReportRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get base report requests for equality.
        /// </summary>
        /// <param name="Object">A get base report request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetBaseReportRequest getBaseReportRequest &&
                   Equals(getBaseReportRequest);

        #endregion

        #region Equals(GetBaseReportRequest)

        /// <summary>
        /// Compares two get base report requests for equality.
        /// </summary>
        /// <param name="GetBaseReportRequest">A get base report request to compare with.</param>
        public override Boolean Equals(GetBaseReportRequest? GetBaseReportRequest)

            => GetBaseReportRequest is not null &&

               GetBaseReportRequestId.Equals(GetBaseReportRequest.GetBaseReportRequestId) &&
               ReportBase.            Equals(GetBaseReportRequest.ReportBase)             &&

               base.           GenericEquals(GetBaseReportRequest);

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

                return GetBaseReportRequestId.GetHashCode() * 5 ^
                       ReportBase.            GetHashCode() * 3 ^

                       base.                  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ReportBase,
                             " /  ",
                             GetBaseReportRequestId.ToString());

        #endregion

    }

}
