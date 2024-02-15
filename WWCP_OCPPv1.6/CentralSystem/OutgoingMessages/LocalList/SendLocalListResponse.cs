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
    /// A send local list response.
    /// </summary>
    public class SendLocalListResponse : AResponse<CS.SendLocalListRequest,
                                                      SendLocalListResponse>,
                                         IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/sendLocalListResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the send local list command.
        /// </summary>
        public UpdateStatus   Status    { get; }

        #endregion

        #region Constructor(s)

        #region SendLocalListResponse(Request, Status)

        /// <summary>
        /// Create a new send local list response.
        /// </summary>
        /// <param name="Request">The send local list request leading to this response.</param>
        /// <param name="Status">The success or failure of the send local list command.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SendLocalListResponse(CS.SendLocalListRequest       Request,
                                     UpdateStatus                  Status,

                                     DateTime?                     ResponseTimestamp   = null,

                                     IEnumerable<KeyPair>?         SignKeys            = null,
                                     IEnumerable<SignInfo>?        SignInfos           = null,
                                     IEnumerable<OCPP.Signature>?  Signatures          = null,

                                     CustomData?                   CustomData          = null)

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

        #region SendLocalListResponse(Request, Result)

        /// <summary>
        /// Create a new send local list response.
        /// </summary>
        /// <param name="Request">The send local list request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public SendLocalListResponse(CS.SendLocalListRequest  Request,
                                     Result                   Result)

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
        //       <ns:sendLocalListResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:sendLocalListResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:SendLocalListResponse",
        //     "title":   "SendLocalListResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Failed",
        //                 "NotSupported",
        //                 "VersionMismatch"
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
        /// Parse the given XML representation of a send local list response.
        /// </summary>
        /// <param name="Request">The send local list request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static SendLocalListResponse Parse(CS.SendLocalListRequest  Request,
                                                  XElement                 XML)
        {

            if (TryParse(Request,
                         XML,
                         out var sendLocalListResponse,
                         out var errorResponse) &&
                sendLocalListResponse is not null)
            {
                return sendLocalListResponse;
            }

            throw new ArgumentException("The given XML representation of a send local list response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomSendLocalListResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a send local list response.
        /// </summary>
        /// <param name="Request">The send local list request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSendLocalListResponseParser">An optional delegate to parse custom send local list responses.</param>
        public static SendLocalListResponse Parse(CS.SendLocalListRequest                              Request,
                                                  JObject                                              JSON,
                                                  CustomJObjectParserDelegate<SendLocalListResponse>?  CustomSendLocalListResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var sendLocalListResponse,
                         out var errorResponse,
                         CustomSendLocalListResponseParser) &&
                sendLocalListResponse is not null)
            {
                return sendLocalListResponse;
            }

            throw new ArgumentException("The given JSON representation of a send local list response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out SendLocalListResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a send local list response.
        /// </summary>
        /// <param name="Request">The send local list request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="SendLocalListResponse">The parsed send local list response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.SendLocalListRequest     Request,
                                       XElement                    XML,
                                       out SendLocalListResponse?  SendLocalListResponse,
                                       out String?                 ErrorResponse)
        {

            try
            {

                SendLocalListResponse = new SendLocalListResponse(

                                            Request,

                                            XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                               UpdateStatusExtensions.Parse)

                                        );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SendLocalListResponse  = null;
                ErrorResponse          = "The given XML representation of a send local list response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SendLocalListResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a send local list response.
        /// </summary>
        /// <param name="Request">The send local list request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SendLocalListResponse">The parsed send local list response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSendLocalListResponseParser">An optional delegate to parse custom send local list responses.</param>
        public static Boolean TryParse(CS.SendLocalListRequest                              Request,
                                       JObject                                              JSON,
                                       out SendLocalListResponse?                           SendLocalListResponse,
                                       out String?                                          ErrorResponse,
                                       CustomJObjectParserDelegate<SendLocalListResponse>?  CustomSendLocalListResponseParser   = null)
        {

            try
            {

                SendLocalListResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "update status",
                                       UpdateStatusExtensions.Parse,
                                       out UpdateStatus Status,
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


                SendLocalListResponse = new SendLocalListResponse(

                                            Request,
                                            Status,
                                            null,

                                            null,
                                            null,
                                            Signatures,

                                            CustomData

                                        );

                if (CustomSendLocalListResponseParser is not null)
                    SendLocalListResponse = CustomSendLocalListResponseParser(JSON,
                                                                              SendLocalListResponse);

                return true;

            }
            catch (Exception e)
            {
                SendLocalListResponse  = null;
                ErrorResponse          = "The given JSON representation of a send local list response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "sendLocalListResponse",

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())

               );

        #endregion

        #region ToJSON(CustomSendLocalListResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSendLocalListResponseSerializer">A delegate to serialize custom send local list responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SendLocalListResponse>?  CustomSendLocalListResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?         CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
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

            return CustomSendLocalListResponseSerializer is not null
                       ? CustomSendLocalListResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The send local list command failed.
        /// </summary>
        /// <param name="Request">The send local list request leading to this response.</param>
        public static SendLocalListResponse Failed(CS.SendLocalListRequest  Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (SendLocalListResponse1, SendLocalListResponse2)

        /// <summary>
        /// Compares two send local list responses for equality.
        /// </summary>
        /// <param name="SendLocalListResponse1">A send local list response.</param>
        /// <param name="SendLocalListResponse2">Another send local list response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SendLocalListResponse? SendLocalListResponse1,
                                           SendLocalListResponse? SendLocalListResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SendLocalListResponse1, SendLocalListResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SendLocalListResponse1 is null || SendLocalListResponse2 is null)
                return false;

            return SendLocalListResponse1.Equals(SendLocalListResponse2);

        }

        #endregion

        #region Operator != (SendLocalListResponse1, SendLocalListResponse2)

        /// <summary>
        /// Compares two send local list responses for inequality.
        /// </summary>
        /// <param name="SendLocalListResponse1">A send local list response.</param>
        /// <param name="SendLocalListResponse2">Another send local list response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SendLocalListResponse? SendLocalListResponse1,
                                           SendLocalListResponse? SendLocalListResponse2)

            => !(SendLocalListResponse1 == SendLocalListResponse2);

        #endregion

        #endregion

        #region IEquatable<SendLocalListResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two send local list responses for equality.
        /// </summary>
        /// <param name="Object">A send local list response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SendLocalListResponse sendLocalListResponse &&
                   Equals(sendLocalListResponse);

        #endregion

        #region Equals(SendLocalListResponse)

        /// <summary>
        /// Compares two send local list responses for equality.
        /// </summary>
        /// <param name="SendLocalListResponse">A send local list response to compare with.</param>
        public override Boolean Equals(SendLocalListResponse? SendLocalListResponse)

            => SendLocalListResponse is not null &&
                   Status.Equals(SendLocalListResponse.Status);

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
