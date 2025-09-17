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
    /// A remote start transaction response.
    /// </summary>
    public class RemoteStartTransactionResponse : AResponse<RemoteStartTransactionRequest,
                                                            RemoteStartTransactionResponse>,
                                                  IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/remoteStartTransactionResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext          Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The status indicating whether the charge point accepts the request to start a charging transaction.
        /// </summary>
        public RemoteStartStopStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new remote start transaction response.
        /// </summary>
        /// <param name="Request">The remote start transaction request leading to this response.</param>
        /// <param name="Status">The status indicating whether the charge point accepts the request to start a charging transaction.</param>
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
        public RemoteStartTransactionResponse(RemoteStartTransactionRequest  Request,
                                              RemoteStartStopStatus          Status,

                                              Result?                        Result                = null,
                                              DateTimeOffset?                ResponseTimestamp     = null,

                                              SourceRouting?                 Destination           = null,
                                              NetworkPath?                   NetworkPath           = null,

                                              IEnumerable<KeyPair>?          SignKeys              = null,
                                              IEnumerable<SignInfo>?         SignInfos             = null,
                                              IEnumerable<Signature>?        Signatures            = null,

                                              CustomData?                    CustomData            = null,

                                              SerializationFormats?          SerializationFormat   = null,
                                              CancellationToken              CancellationToken     = default)

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
        //       <ns:remoteStopTransactionResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:remoteStopTransactionResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:RemoteStartTransactionResponse",
        //     "title":   "RemoteStartTransactionResponse",
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
        /// Parse the given XML representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The remote start transaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static RemoteStartTransactionResponse Parse(RemoteStartTransactionRequest  Request,
                                                           XElement                       XML,
                                                           SourceRouting                  Destination,
                                                           NetworkPath                    NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var remoteStartTransactionResponse,
                         out var errorResponse))
            {
                return remoteStartTransactionResponse;
            }

            throw new ArgumentException("The given XML representation of a remote start transaction response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The RemoteStartTransaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomRemoteStartTransactionResponseParser">An optional delegate to parse custom RemoteStartTransaction responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static RemoteStartTransactionResponse Parse(RemoteStartTransactionRequest                                 Request,
                                                           JObject                                                       JSON,
                                                           SourceRouting                                                 Destination,
                                                           NetworkPath                                                   NetworkPath,
                                                           DateTimeOffset?                                               ResponseTimestamp                            = null,
                                                           CustomJObjectParserDelegate<RemoteStartTransactionResponse>?  CustomRemoteStartTransactionResponseParser   = null,
                                                           CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                                           CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var remoteStartTransactionResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomRemoteStartTransactionResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return remoteStartTransactionResponse;
            }

            throw new ArgumentException("The given JSON representation of a remote start transaction response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out RemoteStartTransactionResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The remote start transaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="RemoteStartTransactionResponse">The parsed remote start transaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(RemoteStartTransactionRequest                             Request,
                                       XElement                                                  XML,
                                       SourceRouting                                             Destination,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out RemoteStartTransactionResponse?  RemoteStartTransactionResponse,
                                       [NotNullWhen(false)] out String?                          ErrorResponse)
        {

            try
            {

                RemoteStartTransactionResponse = new RemoteStartTransactionResponse(

                                                     Request,

                                                     XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                        RemoteStartStopStatusExtensions.Parse)

                                                 );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                RemoteStartTransactionResponse  = null;
                ErrorResponse                   = "The given XML representation of a remote start transaction response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out RemoteStartTransactionResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The RemoteStartTransaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="RemoteStartTransactionResponse">The parsed RemoteStartTransaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomRemoteStartTransactionResponseParser">An optional delegate to parse custom RemoteStartTransaction responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(RemoteStartTransactionRequest                                 Request,
                                       JObject                                                       JSON,
                                       SourceRouting                                                 Destination,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out RemoteStartTransactionResponse?      RemoteStartTransactionResponse,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       DateTimeOffset?                                               ResponseTimestamp                            = null,
                                       CustomJObjectParserDelegate<RemoteStartTransactionResponse>?  CustomRemoteStartTransactionResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                       CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            try
            {

                RemoteStartTransactionResponse = null;

                #region RemoteStartStopStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "remote start stop status",
                                       RemoteStartStopStatusExtensions.Parse,
                                       out RemoteStartStopStatus RemoteStartStopStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures               [optional, OCPP_CSE]

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

                #region CustomData               [optional]

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


                RemoteStartTransactionResponse = new RemoteStartTransactionResponse(

                                                     Request,
                                                     RemoteStartStopStatus,

                                                     null,
                                                     ResponseTimestamp,

                                                     Destination,
                                                     NetworkPath,

                                                     null,
                                                     null,
                                                     Signatures,

                                                     CustomData

                                                 );

                if (CustomRemoteStartTransactionResponseParser is not null)
                    RemoteStartTransactionResponse = CustomRemoteStartTransactionResponseParser(JSON,
                                                                                                RemoteStartTransactionResponse);

                return true;

            }
            catch (Exception e)
            {
                RemoteStartTransactionResponse  = null;
                ErrorResponse                   = "The given JSON representation of a remote start transaction response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "remoteStartTransactionResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomRemoteStartTransactionResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoteStartTransactionResponseSerializer">A delegate to serialize custom remote start transaction responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoteStartTransactionResponse>?  CustomRemoteStartTransactionResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
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

            return CustomRemoteStartTransactionResponseSerializer is not null
                       ? CustomRemoteStartTransactionResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The RemoteStartTransaction failed because of a request error.
        /// </summary>
        /// <param name="Request">The RemoteStartTransaction request.</param>
        public static RemoteStartTransactionResponse RequestError(RemoteStartTransactionRequest  Request,
                                                                  EventTracking_Id               EventTrackingId,
                                                                  ResultCode                     ErrorCode,
                                                                  String?                        ErrorDescription    = null,
                                                                  JObject?                       ErrorDetails        = null,
                                                                  DateTimeOffset?                ResponseTimestamp   = null,

                                                                  SourceRouting?                 Destination         = null,
                                                                  NetworkPath?                   NetworkPath         = null,

                                                                  IEnumerable<KeyPair>?          SignKeys            = null,
                                                                  IEnumerable<SignInfo>?         SignInfos           = null,
                                                                  IEnumerable<Signature>?        Signatures          = null,

                                                                  CustomData?                    CustomData          = null)

            => new (

                   Request,
                   RemoteStartStopStatus.Rejected,
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
        /// The RemoteStartTransaction failed.
        /// </summary>
        /// <param name="Request">The RemoteStartTransaction request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static RemoteStartTransactionResponse FormationViolation(RemoteStartTransactionRequest  Request,
                                                                        String                         ErrorDescription)

            => new (Request,
                    RemoteStartStopStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The RemoteStartTransaction failed.
        /// </summary>
        /// <param name="Request">The RemoteStartTransaction request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static RemoteStartTransactionResponse SignatureError(RemoteStartTransactionRequest  Request,
                                                                    String                         ErrorDescription)

            => new (Request,
                    RemoteStartStopStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The RemoteStartTransaction failed.
        /// </summary>
        /// <param name="Request">The RemoteStartTransaction request.</param>
        /// <param name="Description">An optional error description.</param>
        public static RemoteStartTransactionResponse Failed(RemoteStartTransactionRequest  Request,
                                                            String?                        Description   = null)

            => new (Request,
                    RemoteStartStopStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The RemoteStartTransaction failed because of an exception.
        /// </summary>
        /// <param name="Request">The RemoteStartTransaction request.</param>
        /// <param name="Exception">The exception.</param>
        public static RemoteStartTransactionResponse ExceptionOccurred(RemoteStartTransactionRequest  Request,
                                                                      Exception                      Exception)

            => new (Request,
                    RemoteStartStopStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (RemoteStartTransactionResponse1, RemoteStartTransactionResponse2)

        /// <summary>
        /// Compares two remote start transaction responses for equality.
        /// </summary>
        /// <param name="RemoteStartTransactionResponse1">A remote start transaction response.</param>
        /// <param name="RemoteStartTransactionResponse2">Another remote start transaction response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RemoteStartTransactionResponse? RemoteStartTransactionResponse1,
                                           RemoteStartTransactionResponse? RemoteStartTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RemoteStartTransactionResponse1, RemoteStartTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (RemoteStartTransactionResponse1 is null || RemoteStartTransactionResponse2 is null)
                return false;

            return RemoteStartTransactionResponse1.Equals(RemoteStartTransactionResponse2);

        }

        #endregion

        #region Operator != (RemoteStartTransactionResponse1, RemoteStartTransactionResponse2)

        /// <summary>
        /// Compares two remote start transaction responses for inequality.
        /// </summary>
        /// <param name="RemoteStartTransactionResponse1">A remote start transaction response.</param>
        /// <param name="RemoteStartTransactionResponse2">Another remote start transaction response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoteStartTransactionResponse? RemoteStartTransactionResponse1,
                                           RemoteStartTransactionResponse? RemoteStartTransactionResponse2)

            => !(RemoteStartTransactionResponse1 == RemoteStartTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<RemoteStartTransactionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two remote start transaction responses for equality.
        /// </summary>
        /// <param name="Object">A remote start transaction response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoteStartTransactionResponse remoteStartTransactionResponse &&
                   Equals(remoteStartTransactionResponse);

        #endregion

        #region Equals(RemoteStartTransactionResponse)

        /// <summary>
        /// Compares two remote start transaction responses for equality.
        /// </summary>
        /// <param name="RemoteStartTransactionResponse">A remote start transaction response to compare with.</param>
        public override Boolean Equals(RemoteStartTransactionResponse? RemoteStartTransactionResponse)

            => RemoteStartTransactionResponse is not null &&
                   Status.Equals(RemoteStartTransactionResponse.Status);

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
