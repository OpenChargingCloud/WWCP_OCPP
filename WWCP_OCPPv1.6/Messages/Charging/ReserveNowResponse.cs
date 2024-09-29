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
    /// A ReserveNow response.
    /// </summary>
    public class ReserveNowResponse : AResponse<ReserveNowRequest,
                                                ReserveNowResponse>,
                                      IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/reserveNowResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext      Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the reservation.
        /// </summary>
        [Mandatory]
        public ReservationStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ReserveNow response.
        /// </summary>
        /// <param name="Request">The ReserveNow request leading to this response.</param>
        /// <param name="Status">The success or failure of the reservation.</param>
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
        public ReserveNowResponse(ReserveNowRequest        Request,
                                  ReservationStatus        Status,

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
        //       <ns:reserveNowResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:reserveNowResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ReserveNowResponse",
        //     "title":   "ReserveNowResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Faulted",
        //                 "Occupied",
        //                 "Rejected",
        //                 "Unavailable"
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
        /// Parse the given XML representation of a ReserveNow response.
        /// </summary>
        /// <param name="Request">The ReserveNow request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static ReserveNowResponse Parse(ReserveNowRequest  Request,
                                               XElement           XML,
                                               SourceRouting      Destination,
                                               NetworkPath        NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var reserveNowResponse,
                         out var errorResponse))
            {
                return reserveNowResponse;
            }

            throw new ArgumentException("The given XML representation of a ReserveNow response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a ReserveNow response.
        /// </summary>
        /// <param name="Request">The ReserveNow request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomReserveNowResponseParser">An optional delegate to parse custom ReserveNow responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static ReserveNowResponse Parse(ReserveNowRequest                                 Request,
                                               JObject                                           JSON,
                                               SourceRouting                                     Destination,
                                               NetworkPath                                       NetworkPath,
                                               DateTime?                                         ResponseTimestamp                = null,
                                               CustomJObjectParserDelegate<ReserveNowResponse>?  CustomReserveNowResponseParser   = null,
                                               CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                               CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var reserveNowResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomReserveNowResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return reserveNowResponse;
            }

            throw new ArgumentException("The given JSON representation of a ReserveNow response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out ReserveNowResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a ReserveNow response.
        /// </summary>
        /// <param name="Request">The ReserveNow request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ReserveNowResponse">The parsed ReserveNow response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(ReserveNowRequest                             Request,
                                       XElement                                      XML,
                                       SourceRouting                                 Destination,
                                       NetworkPath                                   NetworkPath,
                                       [NotNullWhen(true)]  out ReserveNowResponse?  ReserveNowResponse,
                                       [NotNullWhen(false)] out String?              ErrorResponse)
        {

            try
            {

                ReserveNowResponse = new ReserveNowResponse(

                                         Request,

                                         XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                            ReservationStatusExtensions.Parse)

                                     );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ReserveNowResponse  = null;
                ErrorResponse       = "The given XML representation of a ReserveNow response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out ReserveNowResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a ReserveNow response.
        /// </summary>
        /// <param name="Request">The ReserveNow request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ReserveNowResponse">The parsed ReserveNow response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomReserveNowResponseParser">An optional delegate to parse custom ReserveNow responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(ReserveNowRequest                                 Request,
                                       JObject                                           JSON,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out ReserveNowResponse?      ReserveNowResponse,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTime?                                         ResponseTimestamp                = null,
                                       CustomJObjectParserDelegate<ReserveNowResponse>?  CustomReserveNowResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                       CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            try
            {

                ReserveNowResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "reservation status",
                                       ReservationStatusExtensions.Parse,
                                       out ReservationStatus Status,
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


                ReserveNowResponse = new ReserveNowResponse(

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

                if (CustomReserveNowResponseParser is not null)
                    ReserveNowResponse = CustomReserveNowResponseParser(JSON,
                                                                        ReserveNowResponse);

                return true;

            }
            catch (Exception e)
            {
                ReserveNowResponse  = null;
                ErrorResponse       = "The given JSON representation of a ReserveNow response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "reserveNowResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomReserveNowResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReserveNowResponseSerializer">A delegate to serialize custom ReserveNow responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ReserveNowResponse>?  CustomReserveNowResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?           CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.AsText()),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomReserveNowResponseSerializer is not null
                       ? CustomReserveNowResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ReserveNow failed because of a request error.
        /// </summary>
        /// <param name="Request">The ReserveNow request.</param>
        public static ReserveNowResponse RequestError(ReserveNowRequest        Request,
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
                   ReservationStatus.Rejected,
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
        /// The ReserveNow failed.
        /// </summary>
        /// <param name="Request">The ReserveNow request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ReserveNowResponse FormationViolation(ReserveNowRequest  Request,
                                                            String             ErrorDescription)

            => new (Request,
                    ReservationStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The ReserveNow failed.
        /// </summary>
        /// <param name="Request">The ReserveNow request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ReserveNowResponse SignatureError(ReserveNowRequest  Request,
                                                        String             ErrorDescription)

            => new (Request,
                    ReservationStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The ReserveNow failed.
        /// </summary>
        /// <param name="Request">The ReserveNow request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ReserveNowResponse Failed(ReserveNowRequest  Request,
                                                String?            Description   = null)

            => new (Request,
                    ReservationStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The ReserveNow failed because of an exception.
        /// </summary>
        /// <param name="Request">The ReserveNow request.</param>
        /// <param name="Exception">The exception.</param>
        public static ReserveNowResponse ExceptionOccured(ReserveNowRequest  Request,
                                                          Exception          Exception)

            => new (Request,
                    ReservationStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ReserveNowResponse1, ReserveNowResponse2)

        /// <summary>
        /// Compares two ReserveNow responses for equality.
        /// </summary>
        /// <param name="ReserveNowResponse1">A ReserveNow response.</param>
        /// <param name="ReserveNowResponse2">Another ReserveNow response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ReserveNowResponse? ReserveNowResponse1,
                                           ReserveNowResponse? ReserveNowResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ReserveNowResponse1, ReserveNowResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ReserveNowResponse1 is null || ReserveNowResponse2 is null)
                return false;

            return ReserveNowResponse1.Equals(ReserveNowResponse2);

        }

        #endregion

        #region Operator != (ReserveNowResponse1, ReserveNowResponse2)

        /// <summary>
        /// Compares two ReserveNow responses for inequality.
        /// </summary>
        /// <param name="ReserveNowResponse1">A ReserveNow response.</param>
        /// <param name="ReserveNowResponse2">Another ReserveNow response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ReserveNowResponse? ReserveNowResponse1,
                                           ReserveNowResponse? ReserveNowResponse2)

            => !(ReserveNowResponse1 == ReserveNowResponse2);

        #endregion

        #endregion

        #region IEquatable<ReserveNowResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ReserveNow responses for equality.
        /// </summary>
        /// <param name="Object">A ReserveNow response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReserveNowResponse reserveNowResponse &&
                   Equals(reserveNowResponse);

        #endregion

        #region Equals(ReserveNowResponse)

        /// <summary>
        /// Compares two ReserveNow responses for equality.
        /// </summary>
        /// <param name="ReserveNowResponse">A ReserveNow response to compare with.</param>
        public override Boolean Equals(ReserveNowResponse? ReserveNowResponse)

            => ReserveNowResponse is not null &&
                   Status.Equals(ReserveNowResponse.Status);

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
