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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The sign certificate request.
    /// </summary>
    public class SignCertificateRequest : ARequest<SignCertificateRequest>
    {

        #region Properties

        /// <summary>
        /// The PEM encoded certificate signing request (CSR) [max 5500].
        /// </summary>
        [Mandatory]
        public String  CSR    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new sign certificate request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CSR">The PEM encoded certificate signing request (CSR) [max 5500].</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public SignCertificateRequest(ChargeBox_Id        ChargeBoxId,
                                      String              CSR,

                                      Request_Id?         RequestId           = null,
                                      DateTime?           RequestTimestamp    = null,
                                      TimeSpan?           RequestTimeout      = null,
                                      EventTracking_Id?   EventTrackingId     = null,
                                      CancellationToken?  CancellationToken   = null)

            : base(ChargeBoxId,
                   "SignCertificate",
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.CSR = CSR;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:SignCertificate.req",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "csr": {
        //       "type": "string",
        //       "maxLength": 5500
        //     }
        //   },
        //   "required": [
        //     "csr"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomSignCertificateRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a sign certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomSignCertificateRequestParser">A delegate to parse custom SignCertificate requests.</param>
        public static SignCertificateRequest Parse(JObject                                               JSON,
                                                   Request_Id                                            RequestId,
                                                   ChargeBox_Id                                          ChargeBoxId,
                                                   CustomJObjectParserDelegate<SignCertificateRequest>?  CustomSignCertificateRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var signCertificateRequest,
                         out var errorResponse,
                         CustomSignCertificateRequestParser))
            {
                return signCertificateRequest!;
            }

            throw new ArgumentException("The given JSON representation of a sign certificate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out SignCertificateRequest, out ErrorResponse, CustomSignCertificateRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a sign certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SignCertificateRequest">The parsed sign certificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                      JSON,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out SignCertificateRequest?  SignCertificateRequest,
                                       out String?                  ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out SignCertificateRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a sign certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SignCertificateRequest">The parsed sign certificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignCertificateRequestParser">A delegate to parse custom sign certificate requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       ChargeBox_Id                                          ChargeBoxId,
                                       out SignCertificateRequest?                           SignCertificateRequest,
                                       out String?                                           ErrorResponse,
                                       CustomJObjectParserDelegate<SignCertificateRequest>?  CustomSignCertificateRequestParser)
        {

            try
            {

                SignCertificateRequest = null;

                #region CSR            [mandatory]

                if (!JSON.ParseMandatoryText("csr",
                                             "certificate signing request",
                                             out String CSR,
                                             out ErrorResponse))
                {
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


                SignCertificateRequest = new SignCertificateRequest(ChargeBoxId,
                                                                    CSR,
                                                                    RequestId);

                if (CustomSignCertificateRequestParser is not null)
                    SignCertificateRequest = CustomSignCertificateRequestParser(JSON,
                                                                                SignCertificateRequest);

                return true;

            }
            catch (Exception e)
            {
                SignCertificateRequest  = null;
                ErrorResponse           = "The given JSON representation of a sign certificate request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignCertificateRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignCertificateRequestSerializer">A delegate to serialize custom sign certificate requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignCertificateRequest>? CustomSignCertificateRequestSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("csr",  CSR)
                       );

            return CustomSignCertificateRequestSerializer is not null
                       ? CustomSignCertificateRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SignCertificateRequest1, SignCertificateRequest2)

        /// <summary>
        /// Compares two sign certificate requests for equality.
        /// </summary>
        /// <param name="SignCertificateRequest1">A sign certificate request.</param>
        /// <param name="SignCertificateRequest2">Another sign certificate request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SignCertificateRequest SignCertificateRequest1,
                                           SignCertificateRequest SignCertificateRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignCertificateRequest1, SignCertificateRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SignCertificateRequest1 is null || SignCertificateRequest2 is null)
                return false;

            return SignCertificateRequest1.Equals(SignCertificateRequest2);

        }

        #endregion

        #region Operator != (SignCertificateRequest1, SignCertificateRequest2)

        /// <summary>
        /// Compares two sign certificate requests for inequality.
        /// </summary>
        /// <param name="SignCertificateRequest1">A sign certificate request.</param>
        /// <param name="SignCertificateRequest2">Another sign certificate request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SignCertificateRequest SignCertificateRequest1,
                                           SignCertificateRequest SignCertificateRequest2)

            => !(SignCertificateRequest1 == SignCertificateRequest2);

        #endregion

        #endregion

        #region IEquatable<SignCertificateRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two sign certificate requests for equality.
        /// </summary>
        /// <param name="Object">A sign certificate request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignCertificateRequest signCertificateRequest &&
                   Equals(signCertificateRequest);

        #endregion

        #region Equals(SignCertificateRequest)

        /// <summary>
        /// Compares two sign certificate requests for equality.
        /// </summary>
        /// <param name="SignCertificateRequest">A sign certificate request to compare with.</param>
        public override Boolean Equals(SignCertificateRequest? SignCertificateRequest)

            => SignCertificateRequest is not null &&

               CSR.        Equals(SignCertificateRequest.CSR) &&

               base.GenericEquals(SignCertificateRequest);

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

                return CSR. GetHashCode() * 3 ^
                       base.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => CSR.SubstringMax(20);

        #endregion

    }

}
