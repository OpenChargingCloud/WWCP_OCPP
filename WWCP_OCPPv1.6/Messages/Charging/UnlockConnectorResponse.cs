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
    /// An UnlockConnector response.
    /// </summary>
    public class UnlockConnectorResponse : AResponse<UnlockConnectorRequest,
                                                     UnlockConnectorResponse>,
                                           IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/unlockConnectorResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the UnlockConnector request.
        /// </summary>
        public UnlockStatus   Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new UnlockConnector response.
        /// </summary>
        /// <param name="Request">The UnlockConnector request leading to this response.</param>
        /// <param name="Status">The success or failure of the UnlockConnector request.</param>
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
        public UnlockConnectorResponse(UnlockConnectorRequest   Request,
                                       UnlockStatus             Status,

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
        //       <ns:unlockConnectorResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:unlockConnectorResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:UnlockConnectorResponse",
        //     "title":   "UnlockConnectorResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Unlocked",
        //                 "UnlockFailed",
        //                 "NotSupported"
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
        /// Parse the given XML representation of an UnlockConnector response.
        /// </summary>
        /// <param name="Request">The UnlockConnector request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static UnlockConnectorResponse Parse(UnlockConnectorRequest  Request,
                                                    XElement                XML,
                                                    SourceRouting           Destination,
                                                    NetworkPath             NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var unlockConnectorResponse,
                         out var errorResponse))
            {
                return unlockConnectorResponse;
            }

            throw new ArgumentException("The given XML representation of an UnlockConnector response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an UnlockConnector response.
        /// </summary>
        /// <param name="Request">The UnlockConnector request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomUnlockConnectorResponseParser">An optional delegate to parse custom UnlockConnector responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static UnlockConnectorResponse Parse(UnlockConnectorRequest                                 Request,
                                                    JObject                                                JSON,
                                                    SourceRouting                                          Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTime?                                              ResponseTimestamp                     = null,
                                                    CustomJObjectParserDelegate<UnlockConnectorResponse>?  CustomUnlockConnectorResponseParser   = null,
                                                    CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                                    CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var unlockConnectorResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomUnlockConnectorResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return unlockConnectorResponse;
            }

            throw new ArgumentException("The given JSON representation of an UnlockConnector response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out UnlockConnectorResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of an UnlockConnector response.
        /// </summary>
        /// <param name="Request">The UnlockConnector request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="UnlockConnectorResponse">The parsed UnlockConnector response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(UnlockConnectorRequest                             Request,
                                       XElement                                           XML,
                                       SourceRouting                                      Destination,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out UnlockConnectorResponse?  UnlockConnectorResponse,
                                       [NotNullWhen(false)] out String?                   ErrorResponse)
        {

            try
            {

                UnlockConnectorResponse = new UnlockConnectorResponse(

                                              Request,

                                              XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                 UnlockStatusExtensions.Parse)

                                          );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorResponse  = null;
                ErrorResponse            = "The given XML representation of an UnlockConnector response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out UnlockConnectorResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an UnlockConnector response.
        /// </summary>
        /// <param name="Request">The UnlockConnector request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="UnlockConnectorResponse">The parsed UnlockConnector response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomUnlockConnectorResponseParser">An optional delegate to parse custom UnlockConnector responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(UnlockConnectorRequest                                 Request,
                                       JObject                                                JSON,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out UnlockConnectorResponse?      UnlockConnectorResponse,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTime?                                              ResponseTimestamp                     = null,
                                       CustomJObjectParserDelegate<UnlockConnectorResponse>?  CustomUnlockConnectorResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                       CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            try
            {

                UnlockConnectorResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "unlock status",
                                       UnlockStatusExtensions.Parse,
                                       out UnlockStatus Status,
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


                UnlockConnectorResponse = new UnlockConnectorResponse(

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

                if (CustomUnlockConnectorResponseParser is not null)
                    UnlockConnectorResponse = CustomUnlockConnectorResponseParser(JSON,
                                                                                  UnlockConnectorResponse);

                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorResponse  = null;
                ErrorResponse            = "The given JSON representation of an UnlockConnector response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "unlockConnectorResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomUnlockConnectorResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnlockConnectorResponseSerializer">A delegate to serialize custom UnlockConnector responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnlockConnectorResponse>?  CustomUnlockConnectorResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
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

            return CustomUnlockConnectorResponseSerializer is not null
                       ? CustomUnlockConnectorResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The UnlockConnector failed because of a request error.
        /// </summary>
        /// <param name="Request">The UnlockConnector request.</param>
        public static UnlockConnectorResponse RequestError(UnlockConnectorRequest   Request,
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
                   UnlockStatus.UnlockFailed,
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
        /// The UnlockConnector failed.
        /// </summary>
        /// <param name="Request">The UnlockConnector request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UnlockConnectorResponse FormationViolation(UnlockConnectorRequest  Request,
                                                                 String                  ErrorDescription)

            => new (Request,
                    UnlockStatus.UnlockFailed,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The UnlockConnector failed.
        /// </summary>
        /// <param name="Request">The UnlockConnector request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UnlockConnectorResponse SignatureError(UnlockConnectorRequest  Request,
                                                             String                  ErrorDescription)

            => new (Request,
                    UnlockStatus.UnlockFailed,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The UnlockConnector failed.
        /// </summary>
        /// <param name="Request">The UnlockConnector request.</param>
        /// <param name="Description">An optional error description.</param>
        public static UnlockConnectorResponse Failed(UnlockConnectorRequest  Request,
                                                     String?                 Description   = null)

            => new (Request,
                    UnlockStatus.UnlockFailed,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The UnlockConnector failed because of an exception.
        /// </summary>
        /// <param name="Request">The UnlockConnector request.</param>
        /// <param name="Exception">The exception.</param>
        public static UnlockConnectorResponse ExceptionOccured(UnlockConnectorRequest  Request,
                                                               Exception               Exception)

            => new (Request,
                    UnlockStatus.UnlockFailed,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (UnlockConnectorResponse1, UnlockConnectorResponse2)

        /// <summary>
        /// Compares two UnlockConnector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse1">An UnlockConnector response.</param>
        /// <param name="UnlockConnectorResponse2">Another UnlockConnector response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnlockConnectorResponse? UnlockConnectorResponse1,
                                           UnlockConnectorResponse? UnlockConnectorResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnlockConnectorResponse1, UnlockConnectorResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UnlockConnectorResponse1 is null || UnlockConnectorResponse2 is null)
                return false;

            return UnlockConnectorResponse1.Equals(UnlockConnectorResponse2);

        }

        #endregion

        #region Operator != (UnlockConnectorResponse1, UnlockConnectorResponse2)

        /// <summary>
        /// Compares two UnlockConnector responses for inequality.
        /// </summary>
        /// <param name="UnlockConnectorResponse1">An UnlockConnector response.</param>
        /// <param name="UnlockConnectorResponse2">Another UnlockConnector response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnlockConnectorResponse? UnlockConnectorResponse1,
                                           UnlockConnectorResponse? UnlockConnectorResponse2)

            => !(UnlockConnectorResponse1 == UnlockConnectorResponse2);

        #endregion

        #endregion

        #region IEquatable<UnlockConnectorResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UnlockConnector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse">An UnlockConnector response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnlockConnectorResponse unlockConnectorResponse &&
                   Equals(unlockConnectorResponse);

        #endregion

        #region Equals(UnlockConnectorResponse)

        /// <summary>
        /// Compares two UnlockConnector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse">An UnlockConnector response to compare with.</param>
        public override Boolean Equals(UnlockConnectorResponse? UnlockConnectorResponse)

            => UnlockConnectorResponse is not null &&
                   Status.Equals(UnlockConnectorResponse.Status);

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
