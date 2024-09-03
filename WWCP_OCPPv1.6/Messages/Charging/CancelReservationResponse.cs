/*
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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A cancel reservation response.
    /// </summary>
    public class CancelReservationResponse : AResponse<CS.CancelReservationRequest,
                                                          CancelReservationResponse>,
                                             IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/cancelReservationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext            Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the reservation cancellation.
        /// </summary>
        public CancelReservationStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region CancelReservationResponse(Request, Status)

        /// <summary>
        /// Create a new cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="Status">The success or failure of the reservation.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public CancelReservationResponse(CS.CancelReservationRequest   Request,
                                         CancelReservationStatus       Status,

                                         DateTime?                     ResponseTimestamp   = null,

                                         IEnumerable<WWCP.KeyPair>?    SignKeys            = null,
                                         IEnumerable<WWCP.SignInfo>?   SignInfos           = null,
                                         IEnumerable<Signature>?  Signatures          = null,

                                         CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status = Status;

        }

        #endregion

        #region CancelReservationResponse(Request, Result)

        /// <summary>
        /// Create a new cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="Result">A result.</param>
        public CancelReservationResponse(CS.CancelReservationRequest  Request,
                                         Result                       Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:cancelReservationStatus>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:cancelReservationStatus>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:CancelReservationResponse",
        //     "title":   "CancelReservationResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static CancelReservationResponse Parse(CS.CancelReservationRequest  Request,
                                                      XElement                     XML)
        {

            if (TryParse(Request,
                         XML,
                         out var cancelReservationResponse,
                         out var errorResponse) &&
                cancelReservationResponse is not null)
            {
                return cancelReservationResponse;
            }

            throw new ArgumentException("The given XML representation of a cancel reservation response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomCancelReservationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCancelReservationResponseParser">An optional delegate to parse custom cancel reservation responses.</param>
        public static CancelReservationResponse Parse(CS.CancelReservationRequest                              Request,
                                                      JObject                                                  JSON,
                                                      CustomJObjectParserDelegate<CancelReservationResponse>?  CustomCancelReservationResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var cancelReservationResponse,
                         out var errorResponse,
                         CustomCancelReservationResponseParser) &&
                cancelReservationResponse is not null)
            {
                return cancelReservationResponse;
            }

            throw new ArgumentException("The given JSON representation of a cancel reservation response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out CancelReservationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="CancelReservationResponse">The parsed cancel reservation response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.CancelReservationRequest     Request,
                                       XElement                        XML,
                                       out CancelReservationResponse?  CancelReservationResponse,
                                       out String?                     ErrorResponse)
        {

            try
            {

                CancelReservationResponse = new CancelReservationResponse(

                                                Request,

                                                XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                   CancelReservationStatusExtensions.Parse)

                                            );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                CancelReservationResponse  = null;
                ErrorResponse              = "The given XML representation of a cancel reservation response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out CancelReservationResponse, out ErrorResponse, CustomCancelReservationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CancelReservationResponse">The parsed cancel reservation response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCancelReservationResponseParser">An optional delegate to parse custom cancel reservation responses.</param>
        public static Boolean TryParse(CS.CancelReservationRequest                              Request,
                                       JObject                                                  JSON,
                                       out CancelReservationResponse?                           CancelReservationResponse,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<CancelReservationResponse>?  CustomCancelReservationResponseParser   = null)
        {

            try
            {

                CancelReservationResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "cancel reservation status",
                                       CancelReservationStatusExtensions.Parse,
                                       out CancelReservationStatus Status,
                                       out ErrorResponse))
                {
                    return false;
                }

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


                CancelReservationResponse = new CancelReservationResponse(

                                                Request,
                                                Status,
                                                null,

                                                null,
                                                null,
                                                Signatures,

                                                CustomData

                                            );

                if (CustomCancelReservationResponseParser is not null)
                    CancelReservationResponse = CustomCancelReservationResponseParser(JSON,
                                                                                      CancelReservationResponse);

                return true;

            }
            catch (Exception e)
            {
                CancelReservationResponse  = null;
                ErrorResponse              = "The given JSON representation of a cancel reservation response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "cancelReservationResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomCancelReservationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCancelReservationResponseSerializer">A delegate to serialize custom cancel reservation responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CancelReservationResponse>?  CustomCancelReservationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCancelReservationResponseSerializer is not null
                       ? CustomCancelReservationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The cancel reservation failed.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        public static CancelReservationResponse Failed(CS.CancelReservationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (CancelReservationResponse1, CancelReservationResponse2)

        /// <summary>
        /// Compares two cancel reservation responses for equality.
        /// </summary>
        /// <param name="CancelReservationResponse1">A cancel reservation response.</param>
        /// <param name="CancelReservationResponse2">Another cancel reservation response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CancelReservationResponse? CancelReservationResponse1,
                                           CancelReservationResponse? CancelReservationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CancelReservationResponse1, CancelReservationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (CancelReservationResponse1 is null || CancelReservationResponse2 is null)
                return false;

            return CancelReservationResponse1.Equals(CancelReservationResponse2);

        }

        #endregion

        #region Operator != (CancelReservationResponse1, CancelReservationResponse2)

        /// <summary>
        /// Compares two cancel reservation responses for inequality.
        /// </summary>
        /// <param name="CancelReservationResponse1">A cancel reservation response.</param>
        /// <param name="CancelReservationResponse2">Another cancel reservation response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CancelReservationResponse? CancelReservationResponse1,
                                           CancelReservationResponse? CancelReservationResponse2)

            => !(CancelReservationResponse1 == CancelReservationResponse2);

        #endregion

        #endregion

        #region IEquatable<CancelReservationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cancel reservation responses for equality.
        /// </summary>
        /// <param name="Object">A cancel reservation response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CancelReservationResponse cancelReservationResponse &&
                   Equals(cancelReservationResponse);

        #endregion

        #region Equals(CancelReservationResponse)

        /// <summary>
        /// Compares two cancel reservation responses for equality.
        /// </summary>
        /// <param name="CancelReservationResponse">A cancel reservation response to compare with.</param>
        public override Boolean Equals(CancelReservationResponse? CancelReservationResponse)

            => CancelReservationResponse is not null &&
                   Status.Equals(CancelReservationResponse.Status);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Status.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Status.ToString();

        #endregion

    }

}
