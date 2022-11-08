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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A certificate signed response.
    /// </summary>
    public class CertificateSignedResponse : AResponse<CS.CertificateSignedRequest,
                                                          CertificateSignedResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the reservation cancellation.
        /// </summary>
        public CertificateSignedStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region CertificateSignedResponse(Request, Status)

        /// <summary>
        /// Create a new certificate signed response.
        /// </summary>
        /// <param name="Request">The certificate signed request leading to this response.</param>
        /// <param name="Status">The success or failure of the reservation.</param>
        public CertificateSignedResponse(CS.CertificateSignedRequest  Request,
                                         CertificateSignedStatus      Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region CertificateSignedResponse(Request, Result)

        /// <summary>
        /// Create a new certificate signed response.
        /// </summary>
        /// <param name="Request">The certificate signed request leading to this response.</param>
        /// <param name="Result">A result.</param>
        public CertificateSignedResponse(CS.CertificateSignedRequest  Request,
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
        //     "id":      "urn:OCPP:1.6:2019:12:CertificateSignedResponse",
        //     "title":   "CertificateSignedResponse",
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
        /// Parse the given XML representation of a certificate signed response.
        /// </summary>
        /// <param name="Request">The certificate signed request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static CertificateSignedResponse Parse(CS.CertificateSignedRequest  Request,
                                                      XElement                     XML)
        {

            if (TryParse(Request,
                         XML,
                         out var cancelReservationResponse,
                         out var errorResponse))
            {
                return cancelReservationResponse!;
            }

            throw new ArgumentException("The given XML representation of a certificate signed response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomCertificateSignedResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a certificate signed response.
        /// </summary>
        /// <param name="Request">The certificate signed request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCertificateSignedResponseParser">A delegate to parse custom certificate signed responses.</param>
        public static CertificateSignedResponse Parse(CS.CertificateSignedRequest                              Request,
                                                      JObject                                                  JSON,
                                                      CustomJObjectParserDelegate<CertificateSignedResponse>?  CustomCertificateSignedResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var cancelReservationResponse,
                         out var errorResponse,
                         CustomCertificateSignedResponseParser))
            {
                return cancelReservationResponse!;
            }

            throw new ArgumentException("The given JSON representation of a certificate signed response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out CertificateSignedResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a certificate signed response.
        /// </summary>
        /// <param name="Request">The certificate signed request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="CertificateSignedResponse">The parsed certificate signed response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.CertificateSignedRequest     Request,
                                       XElement                        XML,
                                       out CertificateSignedResponse?  CertificateSignedResponse,
                                       out String?                     ErrorResponse)
        {

            try
            {

                CertificateSignedResponse = new CertificateSignedResponse(

                                                Request,

                                                XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                   CertificateSignedStatusExtentions.Parse)

                                            );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                CertificateSignedResponse  = null;
                ErrorResponse              = "The given XML representation of a certificate signed response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out CertificateSignedResponse, out ErrorResponse, CustomCertificateSignedResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a certificate signed response.
        /// </summary>
        /// <param name="Request">The certificate signed request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CertificateSignedResponse">The parsed certificate signed response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCertificateSignedResponseParser">A delegate to parse custom certificate signed responses.</param>
        public static Boolean TryParse(CS.CertificateSignedRequest                              Request,
                                       JObject                                                  JSON,
                                       out CertificateSignedResponse?                           CertificateSignedResponse,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<CertificateSignedResponse>?  CustomCertificateSignedResponseParser   = null)
        {

            try
            {

                CertificateSignedResponse = null;

                #region IdTagInfo

                if (!JSON.MapMandatory("status",
                                       "certificate signed status",
                                       CertificateSignedStatusExtentions.Parse,
                                       out CertificateSignedStatus Status,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion


                CertificateSignedResponse = new CertificateSignedResponse(Request,
                                                                          Status);

                if (CustomCertificateSignedResponseParser is not null)
                    CertificateSignedResponse = CustomCertificateSignedResponseParser(JSON,
                                                                                      CertificateSignedResponse);

                return true;

            }
            catch (Exception e)
            {
                CertificateSignedResponse  = null;
                ErrorResponse              = "The given JSON representation of a certificate signed response is invalid: " + e.Message;
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

        #region ToJSON(CustomCertificateSignedResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCertificateSignedResponseSerializer">A delegate to serialize custom certificate signed responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CertificateSignedResponse>? CustomCertificateSignedResponseSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomCertificateSignedResponseSerializer is not null
                       ? CustomCertificateSignedResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The certificate signed failed.
        /// </summary>
        /// <param name="Request">The certificate signed request leading to this response.</param>
        public static CertificateSignedResponse Failed(CS.CertificateSignedRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (CertificateSignedResponse1, CertificateSignedResponse2)

        /// <summary>
        /// Compares two certificate signed responses for equality.
        /// </summary>
        /// <param name="CertificateSignedResponse1">A certificate signed response.</param>
        /// <param name="CertificateSignedResponse2">Another certificate signed response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CertificateSignedResponse CertificateSignedResponse1,
                                           CertificateSignedResponse CertificateSignedResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CertificateSignedResponse1, CertificateSignedResponse2))
                return true;

            // If one is null, but not both, return false.
            if (CertificateSignedResponse1 is null || CertificateSignedResponse2 is null)
                return false;

            return CertificateSignedResponse1.Equals(CertificateSignedResponse2);

        }

        #endregion

        #region Operator != (CertificateSignedResponse1, CertificateSignedResponse2)

        /// <summary>
        /// Compares two certificate signed responses for inequality.
        /// </summary>
        /// <param name="CertificateSignedResponse1">A certificate signed response.</param>
        /// <param name="CertificateSignedResponse2">Another certificate signed response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CertificateSignedResponse CertificateSignedResponse1,
                                           CertificateSignedResponse CertificateSignedResponse2)

            => !(CertificateSignedResponse1 == CertificateSignedResponse2);

        #endregion

        #endregion

        #region IEquatable<CertificateSignedResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two certificate signed responses for equality.
        /// </summary>
        /// <param name="Object">A certificate signed response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateSignedResponse cancelReservationResponse &&
                   Equals(cancelReservationResponse);

        #endregion

        #region Equals(CertificateSignedResponse)

        /// <summary>
        /// Compares two certificate signed responses for equality.
        /// </summary>
        /// <param name="CertificateSignedResponse">A certificate signed response to compare with.</param>
        public override Boolean Equals(CertificateSignedResponse? CertificateSignedResponse)

            => CertificateSignedResponse is not null &&
                   Status.Equals(CertificateSignedResponse.Status);

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
