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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A remote stop transaction response.
    /// </summary>
    public class RemoteStopTransactionResponse : AResponse<CS.RemoteStopTransactionRequest,
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

        #region RemoteStopTransactionResponse(Request, Status)

        /// <summary>
        /// Create a new remote stop transaction response.
        /// </summary>
        /// <param name="Request">The remote stop transaction request leading to this response.</param>
        /// <param name="Status">The status indicating whether the charge point accepts the request to stop the charging transaction.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public RemoteStopTransactionResponse(CS.RemoteStopTransactionRequest  Request,
                                             RemoteStartStopStatus            Status,

                                             DateTime?                        ResponseTimestamp   = null,

                                             IEnumerable<KeyPair>?            SignKeys            = null,
                                             IEnumerable<SignInfo>?           SignInfos           = null,
                                             IEnumerable<OCPP.Signature>?     Signatures          = null,

                                             CustomData?                      CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status = Status;

        }

        #endregion

        #region RemoteStopTransactionResponse(Request, Result)

        /// <summary>
        /// Create a new remote stop transaction response.
        /// </summary>
        /// <param name="Request">The remote stop transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public RemoteStopTransactionResponse(CS.RemoteStopTransactionRequest  Request,
                                             Result                           Result)

            : base(Request,
                   Result)

        { }

        #endregion

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

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a remote stop transaction response.
        /// </summary>
        /// <param name="Request">The remote stop transaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static RemoteStopTransactionResponse Parse(CS.RemoteStopTransactionRequest  Request,
                                                          XElement                         XML)
        {

            if (TryParse(Request,
                         XML,
                         out var remoteStopTransactionResponse,
                         out var errorResponse) &&
                remoteStopTransactionResponse is not null)
            {
                return remoteStopTransactionResponse;
            }

            throw new ArgumentException("The given XML representation of a remote stop transaction response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomRemoteStopTransactionResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a remote stop transaction response.
        /// </summary>
        /// <param name="Request">The remote stop transaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomRemoteStopTransactionResponseParser">A delegate to parse custom remote stop transaction responses.</param>
        public static RemoteStopTransactionResponse Parse(CS.RemoteStopTransactionRequest                              Request,
                                                          JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<RemoteStopTransactionResponse>?  CustomRemoteStopTransactionResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var remoteStopTransactionResponse,
                         out var errorResponse,
                         CustomRemoteStopTransactionResponseParser) &&
                remoteStopTransactionResponse is not null)
            {
                return remoteStopTransactionResponse;
            }

            throw new ArgumentException("The given JSON representation of a remote stop transaction response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out RemoteStopTransactionResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a remote stop transaction response.
        /// </summary>
        /// <param name="Request">The remote stop transaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RemoteStopTransactionResponse">The parsed remote stop transaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.RemoteStopTransactionRequest     Request,
                                       XElement                            XML,
                                       out RemoteStopTransactionResponse?  RemoteStopTransactionResponse,
                                       out String?                         ErrorResponse)
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
                ErrorResponse                  = "The given XML representation of a remote stop transaction response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out RemoteStopTransactionResponse, out ErrorResponse, CustomRemoteStopTransactionResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a remote stop transaction response.
        /// </summary>
        /// <param name="Request">The remote stop transaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RemoteStopTransactionResponse">The parsed remote stop transaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRemoteStopTransactionResponseParser">A delegate to parse custom remote stop transaction responses.</param>
        public static Boolean TryParse(CS.RemoteStopTransactionRequest                              Request,
                                       JObject                                                      JSON,
                                       out RemoteStopTransactionResponse?                           RemoteStopTransactionResponse,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<RemoteStopTransactionResponse>?  CustomRemoteStopTransactionResponseParser   = null)
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


                RemoteStopTransactionResponse = new RemoteStopTransactionResponse(

                                                    Request,
                                                    Status,
                                                    null,

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
                ErrorResponse                  = "The given JSON representation of a remote stop transaction response is invalid: " + e.Message;
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
        /// <param name="CustomRemoteStopTransactionResponseSerializer">A delegate to serialize custom remote stop transaction responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoteStopTransactionResponse>?  CustomRemoteStopTransactionResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                 CustomSignatureSerializer                       = null,
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
        /// The stop transaction failed.
        /// </summary>
        /// <param name="Request">The remote stop transaction request leading to this response.</param>
        public static RemoteStopTransactionResponse Failed(CS.RemoteStopTransactionRequest  Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (RemoteStopTransactionResponse1, RemoteStopTransactionResponse2)

        /// <summary>
        /// Compares two remote stop transaction responses for equality.
        /// </summary>
        /// <param name="RemoteStopTransactionResponse1">A remote stop transaction response.</param>
        /// <param name="RemoteStopTransactionResponse2">Another remote stop transaction response.</param>
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
        /// Compares two remote stop transaction responses for inequality.
        /// </summary>
        /// <param name="RemoteStopTransactionResponse1">A remote stop transaction response.</param>
        /// <param name="RemoteStopTransactionResponse2">Another remote stop transaction response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoteStopTransactionResponse? RemoteStopTransactionResponse1,
                                           RemoteStopTransactionResponse? RemoteStopTransactionResponse2)

            => !(RemoteStopTransactionResponse1 == RemoteStopTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<RemoteStopTransactionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two remote stop transaction responses for equality.
        /// </summary>
        /// <param name="Object">A remote stop transaction response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoteStopTransactionResponse remoteStopTransactionResponse &&
                   Equals(remoteStopTransactionResponse);

        #endregion

        #region Equals(RemoteStopTransactionResponse)

        /// <summary>
        /// Compares two remote stop transaction responses for equality.
        /// </summary>
        /// <param name="RemoteStopTransactionResponse">A remote stop transaction response to compare with.</param>
        public override Boolean Equals(RemoteStopTransactionResponse? RemoteStopTransactionResponse)

            => RemoteStopTransactionResponse is not null &&
                   Status.Equals(RemoteStopTransactionResponse.Status);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Status.GetHashCode();

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
