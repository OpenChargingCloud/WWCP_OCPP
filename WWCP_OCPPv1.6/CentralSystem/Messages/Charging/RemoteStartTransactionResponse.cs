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
    /// A remote start transaction response.
    /// </summary>
    public class RemoteStartTransactionResponse : AResponse<CS.RemoteStartTransactionRequest,
                                                               RemoteStartTransactionResponse>,
                                                  IResponse
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

        #region RemoteStartTransactionResponse(Request, Status)

        /// <summary>
        /// Create a new remote start transaction response.
        /// </summary>
        /// <param name="Request">The remote start transaction request leading to this response.</param>
        /// <param name="Status">The status indicating whether the charge point accepts the request to start a charging transaction.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public RemoteStartTransactionResponse(CS.RemoteStartTransactionRequest  Request,
                                              RemoteStartStopStatus             Status,

                                              DateTime?                         ResponseTimestamp   = null,

                                              IEnumerable<KeyPair>?             SignKeys            = null,
                                              IEnumerable<SignInfo>?            SignInfos           = null,
                                              IEnumerable<OCPP.Signature>?      Signatures          = null,

                                              CustomData?                       CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status = Status;

        }

        #endregion

        #region RemoteStartTransactionResponse(Request, Result)

        /// <summary>
        /// Create a new remote start transaction response.
        /// </summary>
        /// <param name="Request">The remote start transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public RemoteStartTransactionResponse(CS.RemoteStartTransactionRequest  Request,
                                              Result                            Result)

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

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The remote start transaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static RemoteStartTransactionResponse Parse(CS.RemoteStartTransactionRequest  Request,
                                                           XElement                          XML)
        {

            if (TryParse(Request,
                         XML,
                         out var remoteStartTransactionResponse,
                         out var errorResponse) &&
                remoteStartTransactionResponse is not null)
            {
                return remoteStartTransactionResponse;
            }

            throw new ArgumentException("The given XML representation of a remote start transaction response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomRemoteStartTransactionResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The remote start transaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomRemoteStartTransactionResponseParser">A delegate to parse custom remote start transaction responses.</param>
        public static RemoteStartTransactionResponse Parse(CS.RemoteStartTransactionRequest                              Request,
                                                           JObject                                                       JSON,
                                                           CustomJObjectParserDelegate<RemoteStartTransactionResponse>?  CustomRemoteStartTransactionResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var remoteStartTransactionResponse,
                         out var errorResponse,
                         CustomRemoteStartTransactionResponseParser) &&
                remoteStartTransactionResponse is not null)
            {
                return remoteStartTransactionResponse;
            }

            throw new ArgumentException("The given JSON representation of a remote start transaction response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out RemoteStartTransactionResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The remote start transaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RemoteStartTransactionResponse">The parsed remote start transaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.RemoteStartTransactionRequest     Request,
                                       XElement                             XML,
                                       out RemoteStartTransactionResponse?  RemoteStartTransactionResponse,
                                       out String?                          ErrorResponse)
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

        #region (static) TryParse(Request, JSON, out RemoteStartTransactionResponse, out ErrorResponse, CustomRemoteStartTransactionResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The remote start transaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RemoteStartTransactionResponse">The parsed remote start transaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRemoteStartTransactionResponseParser">A delegate to parse custom remote start transaction responses.</param>
        public static Boolean TryParse(CS.RemoteStartTransactionRequest                              Request,
                                       JObject                                                       JSON,
                                       out RemoteStartTransactionResponse?                           RemoteStartTransactionResponse,
                                       out String?                                                   ErrorResponse,
                                       CustomJObjectParserDelegate<RemoteStartTransactionResponse>?  CustomRemoteStartTransactionResponseParser   = null)
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
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData               [optional]

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


                RemoteStartTransactionResponse = new RemoteStartTransactionResponse(

                                                     Request,
                                                     RemoteStartStopStatus,
                                                     null,

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
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                  CustomSignatureSerializer                        = null,
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
        /// The stop transaction failed.
        /// </summary>
        /// <param name="Request">The remote start transaction request leading to this response.</param>
        public static RemoteStartTransactionResponse Failed(CS.RemoteStartTransactionRequest  Request)

            => new (Request,
                    Result.Server());

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
