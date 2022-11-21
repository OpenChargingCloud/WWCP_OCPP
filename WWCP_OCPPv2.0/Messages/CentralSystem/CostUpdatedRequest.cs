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
    /// The cost updated request.
    /// </summary>
    public class CostUpdatedRequest : ARequest<CostUpdatedRequest>
    {

        #region Properties

        /// <summary>
        /// The current total cost, based on the information known by the CSMS, of the transaction including taxes.
        /// In the currency configured with the configuration Variable: [Currency]
        /// </summary>
        [Mandatory]
        public Decimal         TotalCost        { get; }

        /// <summary>
        /// The unique transaction identification the costs are asked for.
        /// </summary>
        [Mandatory]
        public Transaction_Id  TransactionId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new cost updated request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="TotalCost">The current total cost, based on the information known by the CSMS, of the transaction including taxes. In the currency configured with the configuration Variable: [Currency]</param>
        /// <param name="TransactionId">The unique transaction identification the costs are asked for.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public CostUpdatedRequest(ChargeBox_Id        ChargeBoxId,

                                  Decimal             TotalCost,
                                  Transaction_Id      TransactionId,

                                  CustomData?         CustomData          = null,
                                  Request_Id?         RequestId           = null,
                                  DateTime?           RequestTimestamp    = null,
                                  TimeSpan?           RequestTimeout      = null,
                                  EventTracking_Id?   EventTrackingId     = null,
                                  CancellationToken?  CancellationToken   = null)

            : base(ChargeBoxId,
                   "CostUpdated",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.TotalCost      = TotalCost;
            this.TransactionId  = TransactionId;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:CostUpdatedRequest",
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
        //     "totalCost": {
        //       "description": "Current total cost, based on the information known by the CSMS, of the transaction including taxes. In the currency configured with the configuration Variable: [&lt;&lt;configkey-currency, Currency&gt;&gt;]\r\n\r\n",
        //       "type": "number"
        //     },
        //     "transactionId": {
        //       "description": "Transaction Id of the transaction the current cost are asked for.\r\n\r\n",
        //       "type": "string",
        //       "maxLength": 36
        //     }
        //   },
        //   "required": [
        //     "totalCost",
        //     "transactionId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomCostUpdatedRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cost updated request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomCostUpdatedRequestParser">A delegate to parse custom cost updated requests.</param>
        public static CostUpdatedRequest Parse(JObject                                           JSON,
                                               Request_Id                                        RequestId,
                                               ChargeBox_Id                                      ChargeBoxId,
                                               CustomJObjectParserDelegate<CostUpdatedRequest>?  CustomCostUpdatedRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var CostUpdatedRequest,
                         out var errorResponse,
                         CustomCostUpdatedRequestParser))
            {
                return CostUpdatedRequest!;
            }

            throw new ArgumentException("The given JSON representation of a cost updated request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out CostUpdatedRequest, out ErrorResponse, CustomCostUpdatedRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a cost updated request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CostUpdatedRequest">The parsed cost updated request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                  JSON,
                                       Request_Id               RequestId,
                                       ChargeBox_Id             ChargeBoxId,
                                       out CostUpdatedRequest?  CostUpdatedRequest,
                                       out String?              ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out CostUpdatedRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a cost updated request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CostUpdatedRequest">The parsed cost updated request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCostUpdatedRequestParser">A delegate to parse custom cost updated requests.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       Request_Id                                        RequestId,
                                       ChargeBox_Id                                      ChargeBoxId,
                                       out CostUpdatedRequest?                           CostUpdatedRequest,
                                       out String?                                       ErrorResponse,
                                       CustomJObjectParserDelegate<CostUpdatedRequest>?  CustomCostUpdatedRequestParser)
        {

            try
            {

                CostUpdatedRequest = null;

                #region TotalCost        [mandatory]

                if (!JSON.ParseMandatory("totalCost",
                                         "total cost",
                                         out Decimal TotalCost,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TransactionId    [mandatory]

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id TransactionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData       [optional]

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

                #region ChargeBoxId      [optional, OCPP_CSE]

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


                CostUpdatedRequest = new CostUpdatedRequest(ChargeBoxId,
                                                            TotalCost,
                                                            TransactionId,
                                                            CustomData,
                                                            RequestId);

                if (CustomCostUpdatedRequestParser is not null)
                    CostUpdatedRequest = CustomCostUpdatedRequestParser(JSON,
                                                                        CostUpdatedRequest);

                return true;

            }
            catch (Exception e)
            {
                CostUpdatedRequest  = null;
                ErrorResponse       = "The given JSON representation of a cost updated request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCostUpdatedRequestSerializer = null, CustomCustomDataResponseSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCostUpdatedRequestSerializer">A delegate to serialize custom cost updated requests.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CostUpdatedRequest>?  CustomCostUpdatedRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataResponseSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("totalCost",      TotalCost),
                                 new JProperty("transactionId",  TransactionId.ToString()),

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomCostUpdatedRequestSerializer is not null
                       ? CustomCostUpdatedRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CostUpdatedRequest1, CostUpdatedRequest2)

        /// <summary>
        /// Compares two cost updated requests for equality.
        /// </summary>
        /// <param name="CostUpdatedRequest1">A cost updated request.</param>
        /// <param name="CostUpdatedRequest2">Another cost updated request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CostUpdatedRequest? CostUpdatedRequest1,
                                           CostUpdatedRequest? CostUpdatedRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CostUpdatedRequest1, CostUpdatedRequest2))
                return true;

            // If one is null, but not both, return false.
            if (CostUpdatedRequest1 is null || CostUpdatedRequest2 is null)
                return false;

            return CostUpdatedRequest1.Equals(CostUpdatedRequest2);

        }

        #endregion

        #region Operator != (CostUpdatedRequest1, CostUpdatedRequest2)

        /// <summary>
        /// Compares two cost updated requests for inequality.
        /// </summary>
        /// <param name="CostUpdatedRequest1">A cost updated request.</param>
        /// <param name="CostUpdatedRequest2">Another cost updated request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CostUpdatedRequest? CostUpdatedRequest1,
                                           CostUpdatedRequest? CostUpdatedRequest2)

            => !(CostUpdatedRequest1 == CostUpdatedRequest2);

        #endregion

        #endregion

        #region IEquatable<CostUpdatedRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cost updated requests for equality.
        /// </summary>
        /// <param name="Object">A cost updated request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CostUpdatedRequest costUpdatedRequest &&
                   Equals(costUpdatedRequest);

        #endregion

        #region Equals(CostUpdatedRequest)

        /// <summary>
        /// Compares two cost updated requests for equality.
        /// </summary>
        /// <param name="CostUpdatedRequest">A cost updated request to compare with.</param>
        public override Boolean Equals(CostUpdatedRequest? CostUpdatedRequest)

            => CostUpdatedRequest is not null &&

               TotalCost.    Equals(CostUpdatedRequest.TotalCost)     &&
               TransactionId.Equals(CostUpdatedRequest.TransactionId) &&

               base.  GenericEquals(CostUpdatedRequest);

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

                return TotalCost.    GetHashCode() * 5 ^
                       TransactionId.GetHashCode() * 3 ^

                       base.         GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(TotalCost,
                             " for ",
                             TransactionId);

        #endregion

    }

}
