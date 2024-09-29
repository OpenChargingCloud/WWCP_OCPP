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
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A GetDiagnostics response.
    /// </summary>
    public class GetDiagnosticsResponse : AResponse<GetDiagnosticsRequest,
                                                    GetDiagnosticsResponse>,
                                          IResponse<Result>
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

        /// <summary>
        /// Create a new GetDiagnostics response.
        /// </summary>
        /// <param name="Request">The GetDiagnostics request leading to this response.</param>
        /// <param name="FileName">The name of the file with diagnostic information that will be uploaded. This field is not present when no diagnostic information is available.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetDiagnosticsResponse(GetDiagnosticsRequest    Request,
                                      String?                  FileName              = null,

                                      Result?                  Result                = null,
                                      DateTime?                ResponseTimestamp     = null,

                                      SourceRouting?           Destination           = null,
                                      NetworkPath?             NetworkPath           = null,

                                      IEnumerable<KeyPair>?    SignKeys              = null,
                                      IEnumerable<SignInfo>?   SignInfos             = null,
                                      IEnumerable<Signature>?  Signatures            = null,

                                      CustomData?              CustomData            = null,

                                      SerializationFormats?    SerializationFormat   = null,
                                      CancellationToken        CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.FileName = FileName ?? "";

            unchecked
            {

                hashCode = this.FileName.GetHashCode() * 3 ^
                           base.         GetHashCode();

            }

        }

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

        #region (static) Parse   (Request, XML,  Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a GetDiagnostics response.
        /// </summary>
        /// <param name="Request">The GetDiagnostics request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static GetDiagnosticsResponse Parse(GetDiagnosticsRequest  Request,
                                                   XElement               XML,
                                                   SourceRouting          Destination,
                                                   NetworkPath            NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var getDiagnosticsResponse,
                         out var errorResponse))
            {
                return getDiagnosticsResponse;
            }

            throw new ArgumentException("The given XML representation of a GetDiagnostics response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetDiagnostics response.
        /// </summary>
        /// <param name="Request">The GetDiagnostics request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomGetDiagnosticsResponseParser">An optional delegate to parse custom GetDiagnostics responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static GetDiagnosticsResponse Parse(GetDiagnosticsRequest                                 Request,
                                                   JObject                                               JSON,
                                                   SourceRouting                                         Destination,
                                                   NetworkPath                                           NetworkPath,
                                                   DateTime?                                             ResponseTimestamp                    = null,
                                                   CustomJObjectParserDelegate<GetDiagnosticsResponse>?  CustomGetDiagnosticsResponseParser   = null,
                                                   CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                                   CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getDiagnosticsResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetDiagnosticsResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getDiagnosticsResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetDiagnostics response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out GetDiagnosticsResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a GetDiagnostics response.
        /// </summary>
        /// <param name="Request">The GetDiagnostics request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="GetDiagnosticsResponse">The parsed GetDiagnostics response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(GetDiagnosticsRequest                             Request,
                                       XElement                                          XML,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out GetDiagnosticsResponse?  GetDiagnosticsResponse,
                                       [NotNullWhen(false)] out String?                  ErrorResponse)
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
                ErrorResponse           = "The given JSON representation of a GetDiagnostics response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out GetDiagnosticsResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetDiagnostics response.
        /// </summary>
        /// <param name="Request">The GetDiagnostics request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="GetDiagnosticsResponse">The parsed GetDiagnostics response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomGetDiagnosticsResponseParser">An optional delegate to parse custom GetDiagnostics responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(GetDiagnosticsRequest                                 Request,
                                       JObject                                               JSON,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out GetDiagnosticsResponse?      GetDiagnosticsResponse,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       DateTime?                                             ResponseTimestamp                    = null,
                                       CustomJObjectParserDelegate<GetDiagnosticsResponse>?  CustomGetDiagnosticsResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                       CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
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


                GetDiagnosticsResponse = new GetDiagnosticsResponse(

                                             Request,
                                             FileName,

                                             null,
                                             ResponseTimestamp,

                                             Destination,
                                             NetworkPath,

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
                ErrorResponse           = "The given JSON representation of a GetDiagnostics response is invalid: " + e.Message;
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
        /// <param name="CustomGetDiagnosticsResponseSerializer">A delegate to serialize custom GetDiagnostics responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetDiagnosticsResponse>?  CustomGetDiagnosticsResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?               CustomSignatureSerializer                = null,
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
        /// The GetDiagnostics failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetDiagnostics request.</param>
        public static GetDiagnosticsResponse RequestError(GetDiagnosticsRequest    Request,
                                                          EventTracking_Id         EventTrackingId,
                                                          ResultCode               ErrorCode,
                                                          String?                  ErrorDescription    = null,
                                                          JObject?                 ErrorDetails        = null,
                                                          DateTime?                ResponseTimestamp   = null,

                                                          SourceRouting?           Destination         = null,
                                                          NetworkPath?             NetworkPath         = null,

                                                          IEnumerable<KeyPair>?    SignKeys            = null,
                                                          IEnumerable<SignInfo>?   SignInfos           = null,
                                                          IEnumerable<Signature>?  Signatures          = null,

                                                          CustomData?              CustomData          = null)

            => new (

                   Request,
                   null,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The GetDiagnostics failed.
        /// </summary>
        /// <param name="Request">The GetDiagnostics request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetDiagnosticsResponse FormationViolation(GetDiagnosticsRequest  Request,
                                                                String                 ErrorDescription)

            => new (Request,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetDiagnostics failed.
        /// </summary>
        /// <param name="Request">The GetDiagnostics request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetDiagnosticsResponse SignatureError(GetDiagnosticsRequest  Request,
                                                            String                 ErrorDescription)

            => new (Request,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetDiagnostics failed.
        /// </summary>
        /// <param name="Request">The GetDiagnostics request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetDiagnosticsResponse Failed(GetDiagnosticsRequest  Request,
                                                    String?                Description   = null)

            => new (Request,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The GetDiagnostics failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetDiagnostics request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetDiagnosticsResponse ExceptionOccured(GetDiagnosticsRequest  Request,
                                                              Exception              Exception)

            => new (Request,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetDiagnosticsResponse1, GetDiagnosticsResponse2)

        /// <summary>
        /// Compares two GetDiagnostics responses for equality.
        /// </summary>
        /// <param name="GetDiagnosticsResponse1">A GetDiagnostics response.</param>
        /// <param name="GetDiagnosticsResponse2">Another GetDiagnostics response.</param>
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
        /// Compares two GetDiagnostics responses for inequality.
        /// </summary>
        /// <param name="GetDiagnosticsResponse1">A GetDiagnostics response.</param>
        /// <param name="GetDiagnosticsResponse2">Another GetDiagnostics response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetDiagnosticsResponse? GetDiagnosticsResponse1,
                                           GetDiagnosticsResponse? GetDiagnosticsResponse2)

            => !(GetDiagnosticsResponse1 == GetDiagnosticsResponse2);

        #endregion

        #endregion

        #region IEquatable<GetDiagnosticsResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetDiagnostics responses for equality.
        /// </summary>
        /// <param name="Object">A GetDiagnostics response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetDiagnosticsResponse getDiagnosticsResponse &&
                   Equals(getDiagnosticsResponse);

        #endregion

        #region Equals(GetDiagnosticsResponse)

        /// <summary>
        /// Compares two GetDiagnostics responses for equality.
        /// </summary>
        /// <param name="GetDiagnosticsResponse">A GetDiagnostics response to compare with.</param>
        public override Boolean Equals(GetDiagnosticsResponse? GetDiagnosticsResponse)

            => GetDiagnosticsResponse is not null &&
                   FileName.Equals(GetDiagnosticsResponse.FileName);

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

            => FileName.IsNotNullOrEmpty()
                   ? FileName
                   : "<no filename>";

        #endregion

    }

}
