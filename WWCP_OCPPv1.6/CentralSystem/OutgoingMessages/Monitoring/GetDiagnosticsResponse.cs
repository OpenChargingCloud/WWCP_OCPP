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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A get diagnostics response.
    /// </summary>
    public class GetDiagnosticsResponse : AResponse<CS.GetDiagnosticsRequest,
                                                       GetDiagnosticsResponse>,
                                          IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/getDiagnosticsResponse");

        /// <summary>
        /// The maximum length of the name of the file with diagnostic information.
        /// </summary>
        public const           UInt32        MaxFileNameLength    = 255;

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The name of the file with diagnostic information that will
        /// be uploaded. This field is not present when no diagnostic
        /// information is available.
        /// </summary>
        public String         FileName    { get; }

        #endregion

        #region Constructor(s)

        #region GetDiagnosticsResponse(Request, FileName = null)

        /// <summary>
        /// Create a new get diagnostics response.
        /// </summary>
        /// <param name="Request">The get diagnostics request leading to this response.</param>
        /// <param name="FileName">The name of the file with diagnostic information that will be uploaded. This field is not present when no diagnostic information is available.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetDiagnosticsResponse(CS.GetDiagnosticsRequest      Request,
                                      String?                       FileName            = null,

                                      DateTime?                     ResponseTimestamp   = null,

                                      IEnumerable<KeyPair>?         SignKeys            = null,
                                      IEnumerable<SignInfo>?        SignInfos           = null,
                                      IEnumerable<OCPP.Signature>?  Signatures          = null,

                                      CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.FileName = FileName ?? "";

        }

        #endregion

        #region GetDiagnosticsResponse(Request, Result)

        /// <summary>
        /// Create a new get diagnostics response.
        /// </summary>
        /// <param name="Request">The get diagnostics request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetDiagnosticsResponse(CS.GetDiagnosticsRequest  Request,
                                      Result                    Result)

            : base(Request,
                   Result)

        {

            this.FileName = "";

        }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:getDiagnosticsResponse>
        //
        //          <ns:fileName>?</ns:fileName>
        //
        //       </ns:getDiagnosticsResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetDiagnosticsResponse",
        //     "title":   "GetDiagnosticsResponse",
        //     "type":    "object",
        //     "properties": {
        //         "fileName": {
        //             "type": "string",
        //             "maxLength": 255
        //         }
        //     },
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a get diagnostics response.
        /// </summary>
        /// <param name="Request">The get diagnostics request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static GetDiagnosticsResponse Parse(CS.GetDiagnosticsRequest  Request,
                                                   XElement                  XML)
        {

            if (TryParse(Request,
                         XML,
                         out var getDiagnosticsResponse,
                         out var errorResponse) &&
                getDiagnosticsResponse is not null)
            {
                return getDiagnosticsResponse;
            }

            throw new ArgumentException("The given XML representation of a get diagnostics response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetDiagnosticsResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get diagnostics response.
        /// </summary>
        /// <param name="Request">The get diagnostics request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetDiagnosticsResponseParser">A delegate to parse custom get diagnostics responses.</param>
        public static GetDiagnosticsResponse Parse(CS.GetDiagnosticsRequest                              Request,
                                                   JObject                                               JSON,
                                                   CustomJObjectParserDelegate<GetDiagnosticsResponse>?  CustomGetDiagnosticsResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getDiagnosticsResponse,
                         out var errorResponse,
                         CustomGetDiagnosticsResponseParser) &&
                getDiagnosticsResponse is not null)
            {
                return getDiagnosticsResponse;
            }

            throw new ArgumentException("The given JSON representation of a get diagnostics response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out GetDiagnosticsResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a get diagnostics response.
        /// </summary>
        /// <param name="Request">The get diagnostics request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="GetDiagnosticsResponse">The parsed get diagnostics response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.GetDiagnosticsRequest     Request,
                                       XElement                     XML,
                                       out GetDiagnosticsResponse?  GetDiagnosticsResponse,
                                       out String?                  ErrorResponse)
        {

            try
            {

                GetDiagnosticsResponse = new GetDiagnosticsResponse(

                                             Request,

                                             XML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CP + "fileName")

                                         );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetDiagnosticsResponse  = null;
                ErrorResponse           = "The given JSON representation of a get diagnostics response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetDiagnosticsResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get diagnostics response.
        /// </summary>
        /// <param name="Request">The get diagnostics request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetDiagnosticsResponse">The parsed get diagnostics response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetDiagnosticsResponseParser">A delegate to parse custom get diagnostics responses.</param>
        public static Boolean TryParse(CS.GetDiagnosticsRequest                              Request,
                                       JObject                                               JSON,
                                       out GetDiagnosticsResponse?                           GetDiagnosticsResponse,
                                       out String?                                           ErrorResponse,
                                       CustomJObjectParserDelegate<GetDiagnosticsResponse>?  CustomGetDiagnosticsResponseParser   = null)
        {

            try
            {

                GetDiagnosticsResponse = null;

                #region FileName      [optional]

                var FileName = JSON.GetString("fileName");

                #endregion

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


                GetDiagnosticsResponse = new GetDiagnosticsResponse(

                                             Request,
                                             FileName,
                                             null,

                                             null,
                                             null,
                                             Signatures,

                                             CustomData

                                         );

                if (CustomGetDiagnosticsResponseParser is not null)
                    GetDiagnosticsResponse = CustomGetDiagnosticsResponseParser(JSON,
                                                                                GetDiagnosticsResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetDiagnosticsResponse  = null;
                ErrorResponse           = "The given JSON representation of a get diagnostics response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "getDiagnosticsResponse",

                   FileName != null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "fileName",  FileName.SubstringMax(MaxFileNameLength))
                       : null

               );

        #endregion

        #region ToJSON(CustomGetDiagnosticsResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetDiagnosticsResponseSerializer">A delegate to serialize custom get diagnostics responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetDiagnosticsResponse>?  CustomGetDiagnosticsResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?          CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                           FileName.IsNotNullOrEmpty()
                               ? new JProperty("fileName",     FileName.SubstringMax(MaxFileNameLength))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetDiagnosticsResponseSerializer is not null
                       ? CustomGetDiagnosticsResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get diagnostics command failed.
        /// </summary>
        /// <param name="Request">The get diagnostics request leading to this response.</param>
        public static GetDiagnosticsResponse Failed(CS.GetDiagnosticsRequest  Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetDiagnosticsResponse1, GetDiagnosticsResponse2)

        /// <summary>
        /// Compares two get diagnostics responses for equality.
        /// </summary>
        /// <param name="GetDiagnosticsResponse1">A get diagnostics response.</param>
        /// <param name="GetDiagnosticsResponse2">Another get diagnostics response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetDiagnosticsResponse? GetDiagnosticsResponse1,
                                           GetDiagnosticsResponse? GetDiagnosticsResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetDiagnosticsResponse1, GetDiagnosticsResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetDiagnosticsResponse1 is null || GetDiagnosticsResponse2 is null)
                return false;

            return GetDiagnosticsResponse1.Equals(GetDiagnosticsResponse2);

        }

        #endregion

        #region Operator != (GetDiagnosticsResponse1, GetDiagnosticsResponse2)

        /// <summary>
        /// Compares two get diagnostics responses for inequality.
        /// </summary>
        /// <param name="GetDiagnosticsResponse1">A get diagnostics response.</param>
        /// <param name="GetDiagnosticsResponse2">Another get diagnostics response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetDiagnosticsResponse? GetDiagnosticsResponse1,
                                           GetDiagnosticsResponse? GetDiagnosticsResponse2)

            => !(GetDiagnosticsResponse1 == GetDiagnosticsResponse2);

        #endregion

        #endregion

        #region IEquatable<GetDiagnosticsResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get diagnostics responses for equality.
        /// </summary>
        /// <param name="Object">A get diagnostics response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetDiagnosticsResponse getDiagnosticsResponse &&
                   Equals(getDiagnosticsResponse);

        #endregion

        #region Equals(GetDiagnosticsResponse)

        /// <summary>
        /// Compares two get diagnostics responses for equality.
        /// </summary>
        /// <param name="GetDiagnosticsResponse">A get diagnostics response to compare with.</param>
        public override Boolean Equals(GetDiagnosticsResponse? GetDiagnosticsResponse)

            => GetDiagnosticsResponse is not null &&
                   FileName.Equals(GetDiagnosticsResponse.FileName);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => FileName.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => FileName.IsNotNullOrEmpty()
                   ? FileName
                   : "<no filename>";

        #endregion

    }

}
