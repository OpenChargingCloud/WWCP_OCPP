﻿/*
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
    /// Get a certificate revocation list from CSMS for the specified certificate.
    /// </summary>
    public class GetCRLRequest : ARequest<GetCRLRequest>
    {

        #region Properties

        /// <summary>
        /// The identification of this request.
        /// </summary>
        [Mandatory]
        public UInt32               GetCRLRequestId        { get; }

        /// <summary>
        /// Certificate hash data.
        /// </summary>
        [Mandatory]
        public CertificateHashData  CertificateHashData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get certificate revocation list request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetCRLRequestId">The identification of this request.</param>
        /// <param name="CertificateHashData">Certificate hash data.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetCRLRequest(ChargeBox_Id         ChargeBoxId,
                             UInt32               GetCRLRequestId,
                             CertificateHashData  CertificateHashData,
                             CustomData?          CustomData                         = null,

                             Request_Id?          RequestId                          = null,
                             DateTime?            RequestTimestamp                   = null,
                             TimeSpan?            RequestTimeout                     = null,
                             EventTracking_Id?    EventTrackingId                    = null,
                             CancellationToken    CancellationToken                  = default)

            : base(ChargeBoxId,
                   "GetCRL",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.GetCRLRequestId      = GetCRLRequestId;
            this.CertificateHashData  = CertificateHashData;

        }

        #endregion


        //ToDo: Update schema documentation after official release of OCPP 2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomGetCRLRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get certificate revocation list request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomGetCRLRequestParser">A delegate to parse custom get certificate revocation list requests.</param>
        public static GetCRLRequest Parse(JObject                                      JSON,
                                          Request_Id                                   RequestId,
                                          ChargeBox_Id                                 ChargeBoxId,
                                          CustomJObjectParserDelegate<GetCRLRequest>?  CustomGetCRLRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var get15118EVCertificateRequest,
                         out var errorResponse,
                         CustomGetCRLRequestParser))
            {
                return get15118EVCertificateRequest!;
            }

            throw new ArgumentException("The given JSON representation of a get certificate revocation list request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out GetCRLRequest, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get certificate revocation list request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetCRLRequest">The parsed GetCRL request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       Request_Id          RequestId,
                                       ChargeBox_Id        ChargeBoxId,
                                       out GetCRLRequest?  GetCRLRequest,
                                       out String?         ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out GetCRLRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get certificate revocation list request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetCRLRequest">The parsed GetCRL request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetCRLRequestParser">A delegate to parse custom GetCRL requests.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       Request_Id                                   RequestId,
                                       ChargeBox_Id                                 ChargeBoxId,
                                       out GetCRLRequest?                           GetCRLRequest,
                                       out String?                                  ErrorResponse,
                                       CustomJObjectParserDelegate<GetCRLRequest>?  CustomGetCRLRequestParser)
        {

            try
            {

                GetCRLRequest = null;

                #region GetCRLRequestId        [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "get certificate revocation list request identification",
                                         out UInt32 GetCRLRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CertificateHashData    [mandatory]

                if (!JSON.ParseMandatoryJSON("certificateHashData",
                                             "certificate hash data",
                                             OCPPv2_1.CertificateHashData.TryParse,
                                             out CertificateHashData? CertificateHashData,
                                             out ErrorResponse) ||
                    CertificateHashData is null)
                {
                    return false;
                }

                #endregion

                #region CustomData             [optional]

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

                #region ChargeBoxId            [optional, OCPP_CSE]

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


                GetCRLRequest = new GetCRLRequest(
                                    ChargeBoxId,
                                    GetCRLRequestId,
                                    CertificateHashData,
                                    CustomData,
                                    RequestId
                                );

                if (CustomGetCRLRequestParser is not null)
                    GetCRLRequest = CustomGetCRLRequestParser(JSON,
                                                              GetCRLRequest);

                return true;

            }
            catch (Exception e)
            {
                GetCRLRequest  = null;
                ErrorResponse  = "The given JSON representation of a get certificate revocation list request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetCRLSerializer = null, CustomCertificateHashDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCRLSerializer">A delegate to serialize custom GetCRL requests.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom certificate hash datas.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetCRLRequest>?        CustomGetCRLSerializer                = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?  CustomCertificateHashDataSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestId",             GetCRLRequestId),
                                 new JProperty("certificateHashData",   CertificateHashData.ToJSON(CustomCertificateHashDataSerializer,
                                                                                                   CustomCustomDataSerializer)),

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetCRLSerializer is not null
                       ? CustomGetCRLSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetCRLRequest1, GetCRLRequest2)

        /// <summary>
        /// Compares two GetCRL requests for equality.
        /// </summary>
        /// <param name="GetCRLRequest1">A GetCRL request.</param>
        /// <param name="GetCRLRequest2">Another GetCRL request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetCRLRequest? GetCRLRequest1,
                                           GetCRLRequest? GetCRLRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetCRLRequest1, GetCRLRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetCRLRequest1 is null || GetCRLRequest2 is null)
                return false;

            return GetCRLRequest1.Equals(GetCRLRequest2);

        }

        #endregion

        #region Operator != (GetCRLRequest1, GetCRLRequest2)

        /// <summary>
        /// Compares two GetCRL requests for inequality.
        /// </summary>
        /// <param name="GetCRLRequest1">A GetCRL request.</param>
        /// <param name="GetCRLRequest2">Another GetCRL request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetCRLRequest? GetCRLRequest1,
                                           GetCRLRequest? GetCRLRequest2)

            => !(GetCRLRequest1 == GetCRLRequest2);

        #endregion

        #endregion

        #region IEquatable<GetCRLRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get certificate revocation list requests for equality.
        /// </summary>
        /// <param name="Object">A get certificate revocation list request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCRLRequest get15118EVCertificateRequest &&
                   Equals(get15118EVCertificateRequest);

        #endregion

        #region Equals(GetCRLRequest)

        /// <summary>
        /// Compares two get certificate revocation list requests for equality.
        /// </summary>
        /// <param name="GetCRLRequest">A get certificate revocation list request to compare with.</param>
        public override Boolean Equals(GetCRLRequest? GetCRLRequest)

            => GetCRLRequest is not null &&

               GetCRLRequestId.    Equals(GetCRLRequest.GetCRLRequestId)     &&
               CertificateHashData.Equals(GetCRLRequest.CertificateHashData) &&

               base.        GenericEquals(GetCRLRequest);

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

                return GetCRLRequestId.    GetHashCode() * 5 ^
                       CertificateHashData.GetHashCode() * 3 ^
                       base.               GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"Id: {GetCRLRequestId}: {CertificateHashData}";

        #endregion


    }

}