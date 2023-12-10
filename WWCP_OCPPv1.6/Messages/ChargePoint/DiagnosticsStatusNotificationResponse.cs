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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A diagnostics status notification response.
    /// </summary>
    public class DiagnosticsStatusNotificationResponse : AResponse<CP.DiagnosticsStatusNotificationRequest,
                                                                   DiagnosticsStatusNotificationResponse>,
                                                         IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/diagnosticsStatusNotificationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        #region DiagnosticsStatusNotificationResponse()

        /// <summary>
        /// Create a new diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        public DiagnosticsStatusNotificationResponse(CP.DiagnosticsStatusNotificationRequest  Request,

                                                     DateTime?                                ResponseTimestamp   = null,

                                                     IEnumerable<KeyPair>?                    SignKeys            = null,
                                                     IEnumerable<SignInfo>?                   SignInfos           = null,
                                                     IEnumerable<OCPP.Signature>?             Signatures          = null,

                                                     CustomData?                              CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #region DiagnosticsStatusNotificationResponse(Result)

        /// <summary>
        /// Create a new diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public DiagnosticsStatusNotificationResponse(CP.DiagnosticsStatusNotificationRequest  Request,
                                                     Result                                   Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:diagnosticsStatusNotificationResponse />
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:DiagnosticsStatusNotificationResponse",
        //     "title":   "DiagnosticsStatusNotificationResponse",
        //     "type":    "object",
        //     "properties": {},
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static DiagnosticsStatusNotificationResponse Parse(CP.DiagnosticsStatusNotificationRequest  Request,
                                                                  XElement                                 XML)
        {

            if (TryParse(Request,
                         XML,
                         out var diagnosticsStatusNotificationResponse,
                         out var errorResponse))
            {
                return diagnosticsStatusNotificationResponse!;
            }

            throw new ArgumentException("The given XML representation of a diagnostics status notification response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomBootNotificationResponseParser">A delegate to parse custom diagnostics status notification responses.</param>
        public static DiagnosticsStatusNotificationResponse Parse(CP.DiagnosticsStatusNotificationRequest                              Request,
                                                                  JObject                                                              JSON,
                                                                  CustomJObjectParserDelegate<DiagnosticsStatusNotificationResponse>?  CustomBootNotificationResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var diagnosticsStatusNotificationResponse,
                         out var errorResponse,
                         CustomBootNotificationResponseParser))
            {
                return diagnosticsStatusNotificationResponse!;
            }

            throw new ArgumentException("The given JSON representation of a diagnostics status notification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out DiagnosticsStatusNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="DiagnosticsStatusNotificationResponse">The parsed diagnostics status notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CP.DiagnosticsStatusNotificationRequest     Request,
                                       XElement                                    XML,
                                       out DiagnosticsStatusNotificationResponse?  DiagnosticsStatusNotificationResponse,
                                       out String?                                 ErrorResponse)
        {

            try
            {

                ErrorResponse                          = null;
                DiagnosticsStatusNotificationResponse  = new DiagnosticsStatusNotificationResponse(Request);

                return true;

            }
            catch (Exception e)
            {
                DiagnosticsStatusNotificationResponse  = null;
                ErrorResponse                          = "The given XML representation of a diagnostics status notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out DiagnosticsStatusNotificationResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DiagnosticsStatusNotificationResponse">The parsed diagnostics status notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBootNotificationResponseParser">A delegate to parse custom diagnostics status notification responses.</param>
        public static Boolean TryParse(CP.DiagnosticsStatusNotificationRequest                              Request,
                                       JObject                                                              JSON,
                                       out DiagnosticsStatusNotificationResponse?                           DiagnosticsStatusNotificationResponse,
                                       out String?                                                          ErrorResponse,
                                       CustomJObjectParserDelegate<DiagnosticsStatusNotificationResponse>?  CustomBootNotificationResponseParser   = null)
        {

            try
            {

                DiagnosticsStatusNotificationResponse  = null;

                #region Signatures    [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                DiagnosticsStatusNotificationResponse = new DiagnosticsStatusNotificationResponse(

                                                            Request,
                                                            null,

                                                            null,
                                                            null,
                                                            Signatures,

                                                            CustomData
                                                        );

                if (CustomBootNotificationResponseParser is not null)
                    DiagnosticsStatusNotificationResponse = CustomBootNotificationResponseParser(JSON,
                                                                                                 DiagnosticsStatusNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                DiagnosticsStatusNotificationResponse  = null;
                ErrorResponse                          = "The given JSON representation of a diagnostics status notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "diagnosticsStatusNotificationResponse");

        #endregion

        #region ToJSON(CustomDiagnosticsStatusNotificationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDiagnosticsStatusNotificationResponseSerializer">A delegate to serialize custom DiagnosticsStatusNotification responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DiagnosticsStatusNotificationResponse>?  CustomDiagnosticsStatusNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                         CustomSignatureSerializer                               = null,
                              CustomJObjectSerializerDelegate<CustomData>?                             CustomCustomDataSerializer                              = null)
        {

            var json = JSONObject.Create(

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.                 ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDiagnosticsStatusNotificationResponseSerializer is not null
                       ? CustomDiagnosticsStatusNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The diagnostics status notification request failed.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        public static DiagnosticsStatusNotificationResponse Failed(CP.DiagnosticsStatusNotificationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse2)

        /// <summary>
        /// Compares two diagnostics status notification responses for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponse1">A diagnostics status notification response.</param>
        /// <param name="DiagnosticsStatusNotificationResponse2">Another diagnostics status notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DiagnosticsStatusNotificationResponse? DiagnosticsStatusNotificationResponse1,
                                           DiagnosticsStatusNotificationResponse? DiagnosticsStatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (DiagnosticsStatusNotificationResponse1 is null || DiagnosticsStatusNotificationResponse2 is null)
                return false;

            return DiagnosticsStatusNotificationResponse1.Equals(DiagnosticsStatusNotificationResponse2);

        }

        #endregion

        #region Operator != (DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse2)

        /// <summary>
        /// Compares two diagnostics status notification responses for inequality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponse1">A diagnostics status notification response.</param>
        /// <param name="DiagnosticsStatusNotificationResponse2">Another diagnostics status notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DiagnosticsStatusNotificationResponse? DiagnosticsStatusNotificationResponse1,
                                           DiagnosticsStatusNotificationResponse? DiagnosticsStatusNotificationResponse2)

            => !(DiagnosticsStatusNotificationResponse1 == DiagnosticsStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<DiagnosticsStatusNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DiagnosticsStatusNotificationResponse diagnosticsStatusNotificationResponse &&
                   Equals(diagnosticsStatusNotificationResponse);

        #endregion

        #region Equals(DiagnosticsStatusNotificationResponse)

        /// <summary>
        /// Compares two diagnostics status notification responses for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponse">A diagnostics status notification response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(DiagnosticsStatusNotificationResponse? DiagnosticsStatusNotificationResponse)

            => DiagnosticsStatusNotificationResponse is not null;

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

            => "DiagnosticsStatusNotificationResponse";

        #endregion

    }

}
