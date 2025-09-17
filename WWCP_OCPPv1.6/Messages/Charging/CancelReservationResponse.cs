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
    /// A CancelReservation response.
    /// </summary>
    public class CancelReservationResponse : AResponse<CancelReservationRequest,
                                                       CancelReservationResponse>,
                                             IResponse<Result>
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

        /// <summary>
        /// Create a new CancelReservation response.
        /// </summary>
        /// <param name="Request">The CancelReservation request leading to this response.</param>
        /// <param name="Status">The success or failure of the reservation.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public CancelReservationResponse(CancelReservationRequest  Request,
                                         CancelReservationStatus   Status,

                                         Result?                   Result                = null,
                                         DateTimeOffset?           ResponseTimestamp     = null,

                                         SourceRouting?            Destination           = null,
                                         NetworkPath?              NetworkPath           = null,

                                         IEnumerable<KeyPair>?     SignKeys              = null,
                                         IEnumerable<SignInfo>?    SignInfos             = null,
                                         IEnumerable<Signature>?   Signatures            = null,

                                         CustomData?               CustomData            = null,

                                         SerializationFormats?     SerializationFormat   = null,
                                         CancellationToken         CancellationToken     = default)

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

        #region (static) Parse   (Request, XML,  Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a CancelReservation response.
        /// </summary>
        /// <param name="Request">The CancelReservation request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static CancelReservationResponse Parse(CancelReservationRequest  Request,
                                                      XElement                  XML,
                                                      SourceRouting             Destination,
                                                      NetworkPath               NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var cancelReservationResponse,
                         out var errorResponse))
            {
                return cancelReservationResponse;
            }

            throw new ArgumentException("The given XML representation of a CancelReservation response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a CancelReservation response.
        /// </summary>
        /// <param name="Request">The CancelReservation request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomCancelReservationResponseParser">An optional delegate to parse custom CancelReservation responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static CancelReservationResponse Parse(CancelReservationRequest                                 Request,
                                                      JObject                                                  JSON,
                                                      SourceRouting                                            Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTimeOffset?                                          ResponseTimestamp                       = null,
                                                      CustomJObjectParserDelegate<CancelReservationResponse>?  CustomCancelReservationResponseParser   = null,
                                                      CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                                      CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var cancelReservationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomCancelReservationResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return cancelReservationResponse;
            }

            throw new ArgumentException("The given JSON representation of a CancelReservation response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out CancelReservationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a CancelReservation response.
        /// </summary>
        /// <param name="Request">The CancelReservation request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="CancelReservationResponse">The parsed CancelReservation response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CancelReservationRequest                             Request,
                                       XElement                                             XML,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out CancelReservationResponse?  CancelReservationResponse,
                                       [NotNullWhen(false)] out String?                     ErrorResponse)
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
                ErrorResponse              = "The given XML representation of a CancelReservation response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out CancelReservationResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a CancelReservation response.
        /// </summary>
        /// <param name="Request">The CancelReservation request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="CancelReservationResponse">The parsed CancelReservation response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomCancelReservationResponseParser">An optional delegate to parse custom CancelReservation responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(CancelReservationRequest                                 Request,
                                       JObject                                                  JSON,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out CancelReservationResponse?      CancelReservationResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTimeOffset?                                          ResponseTimestamp                       = null,
                                       CustomJObjectParserDelegate<CancelReservationResponse>?  CustomCancelReservationResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                       CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            try
            {

                CancelReservationResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "CancelReservation status",
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
                                                ResponseTimestamp,

                                                Destination,
                                                NetworkPath,

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
                ErrorResponse              = "The given JSON representation of a CancelReservation response is invalid: " + e.Message;
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
        /// <param name="CustomCancelReservationResponseSerializer">A delegate to serialize custom CancelReservation responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CancelReservationResponse>?  CustomCancelReservationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
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
        /// The CancelReservation failed because of a request error.
        /// </summary>
        /// <param name="Request">The CancelReservation request.</param>
        public static CancelReservationResponse RequestError(CancelReservationRequest  Request,
                                                             EventTracking_Id          EventTrackingId,
                                                             ResultCode                ErrorCode,
                                                             String?                   ErrorDescription    = null,
                                                             JObject?                  ErrorDetails        = null,
                                                             DateTimeOffset?           ResponseTimestamp   = null,

                                                             SourceRouting?            Destination         = null,
                                                             NetworkPath?              NetworkPath         = null,

                                                             IEnumerable<KeyPair>?     SignKeys            = null,
                                                             IEnumerable<SignInfo>?    SignInfos           = null,
                                                             IEnumerable<Signature>?   Signatures          = null,

                                                             CustomData?               CustomData          = null)

            => new (

                   Request,
                   CancelReservationStatus.Rejected,
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
        /// The CancelReservation failed.
        /// </summary>
        /// <param name="Request">The CancelReservation request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static CancelReservationResponse FormationViolation(CancelReservationRequest  Request,
                                                                   String                    ErrorDescription)

            => new (Request,
                    CancelReservationStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The CancelReservation failed.
        /// </summary>
        /// <param name="Request">The CancelReservation request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static CancelReservationResponse SignatureError(CancelReservationRequest  Request,
                                                               String                    ErrorDescription)

            => new (Request,
                    CancelReservationStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The CancelReservation failed.
        /// </summary>
        /// <param name="Request">The CancelReservation request.</param>
        /// <param name="Description">An optional error description.</param>
        public static CancelReservationResponse Failed(CancelReservationRequest  Request,
                                                       String?                   Description   = null)

            => new (Request,
                    CancelReservationStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The CancelReservation failed because of an exception.
        /// </summary>
        /// <param name="Request">The CancelReservation request.</param>
        /// <param name="Exception">The exception.</param>
        public static CancelReservationResponse ExceptionOccurred(CancelReservationRequest  Request,
                                                                 Exception                 Exception)

            => new (Request,
                    CancelReservationStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (CancelReservationResponse1, CancelReservationResponse2)

        /// <summary>
        /// Compares two CancelReservation responses for equality.
        /// </summary>
        /// <param name="CancelReservationResponse1">A CancelReservation response.</param>
        /// <param name="CancelReservationResponse2">Another CancelReservation response.</param>
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
        /// Compares two CancelReservation responses for inequality.
        /// </summary>
        /// <param name="CancelReservationResponse1">A CancelReservation response.</param>
        /// <param name="CancelReservationResponse2">Another CancelReservation response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CancelReservationResponse? CancelReservationResponse1,
                                           CancelReservationResponse? CancelReservationResponse2)

            => !(CancelReservationResponse1 == CancelReservationResponse2);

        #endregion

        #endregion

        #region IEquatable<CancelReservationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CancelReservation responses for equality.
        /// </summary>
        /// <param name="Object">A CancelReservation response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CancelReservationResponse cancelReservationResponse &&
                   Equals(cancelReservationResponse);

        #endregion

        #region Equals(CancelReservationResponse)

        /// <summary>
        /// Compares two CancelReservation responses for equality.
        /// </summary>
        /// <param name="CancelReservationResponse">A CancelReservation response to compare with.</param>
        public override Boolean Equals(CancelReservationResponse? CancelReservationResponse)

            => CancelReservationResponse is not null &&
                   Status.Equals(CancelReservationResponse.Status);

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
