/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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
    /// A ClearCache response.
    /// </summary>
    public class ClearCacheResponse : AResponse<ClearCacheRequest,
                                                ClearCacheResponse>,
                                      IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/clearCacheResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext     Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the ClearCache command.
        /// </summary>
        public ClearCacheStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearCache response.
        /// </summary>
        /// <param name="Request">The ClearCache request leading to this response.</param>
        /// <param name="Status">The success or failure of the ClearCache command.</param>
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
        public ClearCacheResponse(ClearCacheRequest        Request,
                                  ClearCacheStatus         Status,

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

            this.Status = Status;

            unchecked
            {

                hashCode = this.Status.GetHashCode() * 3 ^
                           base.       GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:clearCacheResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:clearCacheResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (Request, XML,  Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a ClearCache response.
        /// </summary>
        /// <param name="Request">The ClearCache request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static ClearCacheResponse Parse(ClearCacheRequest  Request,
                                               XElement           XML,
                                               SourceRouting      Destination,
                                               NetworkPath        NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var clearCacheResponse,
                         out var errorResponse))
            {
                return clearCacheResponse;
            }

            throw new ArgumentException("The given XML representation of a ClearCache response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a ClearCache response.
        /// </summary>
        /// <param name="Request">The ClearCache request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomClearCacheResponseParser">An optional delegate to parse custom ClearCache responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static ClearCacheResponse Parse(ClearCacheRequest                                 Request,
                                               JObject                                           JSON,
                                               SourceRouting                                     Destination,
                                               NetworkPath                                       NetworkPath,
                                               DateTime?                                         ResponseTimestamp                = null,
                                               CustomJObjectParserDelegate<ClearCacheResponse>?  CustomClearCacheResponseParser   = null,
                                               CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                               CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var clearCacheResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomClearCacheResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return clearCacheResponse;
            }

            throw new ArgumentException("The given JSON representation of a ClearCache response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out ClearCacheResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a ClearCache response.
        /// </summary>
        /// <param name="Request">The ClearCache request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ClearCacheResponse">The parsed ClearCache response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(ClearCacheRequest                             Request,
                                       XElement                                      XML,
                                       SourceRouting                                 Destination,
                                       NetworkPath                                   NetworkPath,
                                       [NotNullWhen(true)]  out ClearCacheResponse?  ClearCacheResponse,
                                       [NotNullWhen(false)] out String?              ErrorResponse)
        {

            try
            {

                ClearCacheResponse = new ClearCacheResponse(

                                         Request,

                                         XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                            ClearCacheStatusExtensions.Parse)

                                     );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ClearCacheResponse  = null;
                ErrorResponse       = "The given XML representation of a ClearCache response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out ClearCacheResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a ClearCache response.
        /// </summary>
        /// <param name="Request">The ClearCache request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ClearCacheResponse">The parsed ClearCache response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomClearCacheResponseParser">An optional delegate to parse custom ClearCache responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(ClearCacheRequest                                 Request,
                                       JObject                                           JSON,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out ClearCacheResponse?      ClearCacheResponse,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTime?                                         ResponseTimestamp                = null,
                                       CustomJObjectParserDelegate<ClearCacheResponse>?  CustomClearCacheResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                       CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            try
            {

                ClearCacheResponse = null;

                #region ClearCacheStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "ClearCache status",
                                       ClearCacheStatusExtensions.Parse,
                                       out ClearCacheStatus ClearCacheStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures          [optional, OCPP_CSE]

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

                #region CustomData          [optional]

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


                ClearCacheResponse = new ClearCacheResponse(

                                         Request,
                                         ClearCacheStatus,

                                         null,
                                         ResponseTimestamp,

                                         Destination,
                                         NetworkPath,

                                         null,
                                         null,
                                         Signatures,

                                         CustomData

                                     );

                if (CustomClearCacheResponseParser is not null)
                    ClearCacheResponse = CustomClearCacheResponseParser(JSON,
                                                                        ClearCacheResponse);

                return true;

            }
            catch (Exception e)
            {
                ClearCacheResponse  = null;
                ErrorResponse       = "The given JSON representation of a ClearCache response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "clearCacheResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomClearCacheResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearCacheResponseSerializer">A delegate to serialize custom ClearCache responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearCacheResponse>?  CustomClearCacheResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?           CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
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

            return CustomClearCacheResponseSerializer is not null
                       ? CustomClearCacheResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ClearCache failed because of a request error.
        /// </summary>
        /// <param name="Request">The ClearCache request.</param>
        public static ClearCacheResponse RequestError(ClearCacheRequest        Request,
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
                   ClearCacheStatus.Rejected,
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
        /// The ClearCache failed.
        /// </summary>
        /// <param name="Request">The ClearCache request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ClearCacheResponse FormationViolation(ClearCacheRequest  Request,
                                                            String             ErrorDescription)

            => new (Request,
                    ClearCacheStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The ClearCache failed.
        /// </summary>
        /// <param name="Request">The ClearCache request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ClearCacheResponse SignatureError(ClearCacheRequest  Request,
                                                        String             ErrorDescription)

            => new (Request,
                    ClearCacheStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The ClearCache failed.
        /// </summary>
        /// <param name="Request">The ClearCache request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ClearCacheResponse Failed(ClearCacheRequest  Request,
                                                String?            Description   = null)

            => new (Request,
                    ClearCacheStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The ClearCache failed because of an exception.
        /// </summary>
        /// <param name="Request">The ClearCache request.</param>
        /// <param name="Exception">The exception.</param>
        public static ClearCacheResponse ExceptionOccurred(ClearCacheRequest  Request,
                                                          Exception          Exception)

            => new (Request,
                    ClearCacheStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ClearCacheResponse1, ClearCacheResponse2)

        /// <summary>
        /// Compares two ClearCache responses for equality.
        /// </summary>
        /// <param name="ClearCacheResponse1">A ClearCache response.</param>
        /// <param name="ClearCacheResponse2">Another ClearCache response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearCacheResponse? ClearCacheResponse1,
                                           ClearCacheResponse? ClearCacheResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearCacheResponse1, ClearCacheResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ClearCacheResponse1 is null || ClearCacheResponse2 is null)
                return false;

            return ClearCacheResponse1.Equals(ClearCacheResponse2);

        }

        #endregion

        #region Operator != (ClearCacheResponse1, ClearCacheResponse2)

        /// <summary>
        /// Compares two ClearCache responses for inequality.
        /// </summary>
        /// <param name="ClearCacheResponse1">A ClearCache response.</param>
        /// <param name="ClearCacheResponse2">Another ClearCache response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearCacheResponse? ClearCacheResponse1,
                                           ClearCacheResponse? ClearCacheResponse2)

            => !(ClearCacheResponse1 == ClearCacheResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearCacheResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearCache responses for equality.
        /// </summary>
        /// <param name="Object">A ClearCache response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearCacheResponse clearCacheResponse &&
                   Equals(clearCacheResponse);

        #endregion

        #region Equals(ClearCacheResponse)

        /// <summary>
        /// Compares two ClearCache responses for equality.
        /// </summary>
        /// <param name="ClearCacheResponse">A ClearCache response to compare with.</param>
        public override Boolean Equals(ClearCacheResponse? ClearCacheResponse)

            => ClearCacheResponse is not null &&
                   Status.Equals(ClearCacheResponse.Status);

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

            => Status.ToString();

        #endregion

    }

}
