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
    /// A RemoteStopTransaction response.
    /// </summary>
    public class RemoteStopTransactionResponse : AResponse<RemoteStopTransactionRequest,
                                                           RemoteStopTransactionResponse>,
                                                 IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/remoteStopTransactionResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext          Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The status indicating whether the charge point accepts the request to stop the charging transaction.
        /// </summary>
        public RemoteStartStopStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new RemoteStopTransaction response.
        /// </summary>
        /// <param name="Request">The RemoteStopTransaction request leading to this response.</param>
        /// <param name="Status">The status indicating whether the charge point accepts the request to stop the charging transaction.</param>
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
        public RemoteStopTransactionResponse(RemoteStopTransactionRequest  Request,
                                             RemoteStartStopStatus         Status,

                                             Result?                       Result                = null,
                                             DateTime?                     ResponseTimestamp     = null,

                                             SourceRouting?                Destination           = null,
                                             NetworkPath?                  NetworkPath           = null,

                                             IEnumerable<KeyPair>?         SignKeys              = null,
                                             IEnumerable<SignInfo>?        SignInfos             = null,
                                             IEnumerable<Signature>?       Signatures            = null,

                                             CustomData?                   CustomData            = null,

                                             SerializationFormats?         SerializationFormat   = null,
                                             CancellationToken             CancellationToken     = default)

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
        //     "id":      "urn:OCPP:1.6:2019:12:RemoteStopTransactionResponse",
        //     "title":   "RemoteStopTransactionResponse",
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
        /// Parse the given XML representation of a RemoteStopTransaction response.
        /// </summary>
        /// <param name="Request">The RemoteStopTransaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static RemoteStopTransactionResponse Parse(RemoteStopTransactionRequest  Request,
                                                          XElement                      XML,
                                                          SourceRouting                 Destination,
                                                          NetworkPath                   NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var remoteStopTransactionResponse,
                         out var errorResponse))
            {
                return remoteStopTransactionResponse;
            }

            throw new ArgumentException("The given XML representation of a RemoteStopTransaction response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a RemoteStopTransaction response.
        /// </summary>
        /// <param name="Request">The RemoteStopTransaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomRemoteStopTransactionResponseParser">An optional delegate to parse custom RemoteStopTransaction responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static RemoteStopTransactionResponse Parse(RemoteStopTransactionRequest                                 Request,
                                                          JObject                                                      JSON,
                                                          SourceRouting                                                Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    ResponseTimestamp                           = null,
                                                          CustomJObjectParserDelegate<RemoteStopTransactionResponse>?  CustomRemoteStopTransactionResponseParser   = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var remoteStopTransactionResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomRemoteStopTransactionResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return remoteStopTransactionResponse;
            }

            throw new ArgumentException("The given JSON representation of a RemoteStopTransaction response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out RemoteStopTransactionResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a RemoteStopTransaction response.
        /// </summary>
        /// <param name="Request">The RemoteStopTransaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="RemoteStopTransactionResponse">The parsed RemoteStopTransaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(RemoteStopTransactionRequest                             Request,
                                       XElement                                                 XML,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out RemoteStopTransactionResponse?  RemoteStopTransactionResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse)
        {

            try
            {

                RemoteStopTransactionResponse = new RemoteStopTransactionResponse(

                                                    Request,

                                                    XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                       RemoteStartStopStatusExtensions.Parse)

                                                );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                RemoteStopTransactionResponse  = null;
                ErrorResponse                  = "The given XML representation of a RemoteStopTransaction response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out RemoteStopTransactionResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a RemoteStopTransaction response.
        /// </summary>
        /// <param name="Request">The RemoteStopTransaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="RemoteStopTransactionResponse">The parsed RemoteStopTransaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomRemoteStopTransactionResponseParser">An optional delegate to parse custom RemoteStopTransaction responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(RemoteStopTransactionRequest                                 Request,
                                       JObject                                                      JSON,
                                       SourceRouting                                                Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out RemoteStopTransactionResponse?      RemoteStopTransactionResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    ResponseTimestamp                           = null,
                                       CustomJObjectParserDelegate<RemoteStopTransactionResponse>?  CustomRemoteStopTransactionResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                RemoteStopTransactionResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "remote start stop status",
                                       RemoteStartStopStatusExtensions.Parse,
                                       out RemoteStartStopStatus Status,
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


                RemoteStopTransactionResponse = new RemoteStopTransactionResponse(

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

                if (CustomRemoteStopTransactionResponseParser is not null)
                    RemoteStopTransactionResponse = CustomRemoteStopTransactionResponseParser(JSON,
                                                                                              RemoteStopTransactionResponse);

                return true;

            }
            catch (Exception e)
            {
                RemoteStopTransactionResponse  = null;
                ErrorResponse                  = "The given JSON representation of a RemoteStopTransaction response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "remoteStopTransactionResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomRemoteStopTransactionResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoteStopTransactionResponseSerializer">A delegate to serialize custom RemoteStopTransaction responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoteStopTransactionResponse>?  CustomRemoteStopTransactionResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
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

            return CustomRemoteStopTransactionResponseSerializer is not null
                       ? CustomRemoteStopTransactionResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The RemoteStopTransaction failed because of a request error.
        /// </summary>
        /// <param name="Request">The RemoteStopTransaction request.</param>
        public static RemoteStopTransactionResponse RequestError(RemoteStopTransactionRequest  Request,
                                                                 EventTracking_Id              EventTrackingId,
                                                                 ResultCode                    ErrorCode,
                                                                 String?                       ErrorDescription    = null,
                                                                 JObject?                      ErrorDetails        = null,
                                                                 DateTime?                     ResponseTimestamp   = null,

                                                                 SourceRouting?                Destination         = null,
                                                                 NetworkPath?                  NetworkPath         = null,

                                                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                                                 IEnumerable<Signature>?       Signatures          = null,

                                                                 CustomData?                   CustomData          = null)

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
        /// The RemoteStopTransaction failed.
        /// </summary>
        /// <param name="Request">The RemoteStopTransaction request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static RemoteStopTransactionResponse FormationViolation(RemoteStopTransactionRequest  Request,
                                                                       String                        ErrorDescription)

            => new (Request,
                    RemoteStartStopStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The RemoteStopTransaction failed.
        /// </summary>
        /// <param name="Request">The RemoteStopTransaction request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static RemoteStopTransactionResponse SignatureError(RemoteStopTransactionRequest  Request,
                                                                   String                        ErrorDescription)

            => new (Request,
                    RemoteStartStopStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The RemoteStopTransaction failed.
        /// </summary>
        /// <param name="Request">The RemoteStopTransaction request.</param>
        /// <param name="Description">An optional error description.</param>
        public static RemoteStopTransactionResponse Failed(RemoteStopTransactionRequest  Request,
                                                           String?                       Description   = null)

            => new (Request,
                    RemoteStartStopStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The RemoteStopTransaction failed because of an exception.
        /// </summary>
        /// <param name="Request">The RemoteStopTransaction request.</param>
        /// <param name="Exception">The exception.</param>
        public static RemoteStopTransactionResponse ExceptionOccured(RemoteStopTransactionRequest  Request,
                                                                     Exception                     Exception)

            => new (Request,
                    RemoteStartStopStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (RemoteStopTransactionResponse1, RemoteStopTransactionResponse2)

        /// <summary>
        /// Compares two RemoteStopTransaction responses for equality.
        /// </summary>
        /// <param name="RemoteStopTransactionResponse1">A RemoteStopTransaction response.</param>
        /// <param name="RemoteStopTransactionResponse2">Another RemoteStopTransaction response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RemoteStopTransactionResponse? RemoteStopTransactionResponse1,
                                           RemoteStopTransactionResponse? RemoteStopTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RemoteStopTransactionResponse1, RemoteStopTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (RemoteStopTransactionResponse1 is null || RemoteStopTransactionResponse2 is null)
                return false;

            return RemoteStopTransactionResponse1.Equals(RemoteStopTransactionResponse2);

        }

        #endregion

        #region Operator != (RemoteStopTransactionResponse1, RemoteStopTransactionResponse2)

        /// <summary>
        /// Compares two RemoteStopTransaction responses for inequality.
        /// </summary>
        /// <param name="RemoteStopTransactionResponse1">A RemoteStopTransaction response.</param>
        /// <param name="RemoteStopTransactionResponse2">Another RemoteStopTransaction response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoteStopTransactionResponse? RemoteStopTransactionResponse1,
                                           RemoteStopTransactionResponse? RemoteStopTransactionResponse2)

            => !(RemoteStopTransactionResponse1 == RemoteStopTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<RemoteStopTransactionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two RemoteStopTransaction responses for equality.
        /// </summary>
        /// <param name="Object">A RemoteStopTransaction response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoteStopTransactionResponse remoteStopTransactionResponse &&
                   Equals(remoteStopTransactionResponse);

        #endregion

        #region Equals(RemoteStopTransactionResponse)

        /// <summary>
        /// Compares two RemoteStopTransaction responses for equality.
        /// </summary>
        /// <param name="RemoteStopTransactionResponse">A RemoteStopTransaction response to compare with.</param>
        public override Boolean Equals(RemoteStopTransactionResponse? RemoteStopTransactionResponse)

            => RemoteStopTransactionResponse is not null &&
                   Status.Equals(RemoteStopTransactionResponse.Status);

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
