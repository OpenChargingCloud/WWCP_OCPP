﻿/*
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
    /// A trigger message response.
    /// </summary>
    public class TriggerMessageResponse : AResponse<CS.TriggerMessageRequest,
                                                       TriggerMessageResponse>,
                                          IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/triggerMessageResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext         Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the trigger message command.
        /// </summary>
        public TriggerMessageStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region TriggerMessageResponse(Request, Status)

        /// <summary>
        /// Create a new trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="Status">The success or failure of the trigger message command.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public TriggerMessageResponse(CS.TriggerMessageRequest      Request,
                                      TriggerMessageStatus          Status,

                                      DateTime?                     ResponseTimestamp   = null,

                                      IEnumerable<KeyPair>?         SignKeys            = null,
                                      IEnumerable<SignInfo>?        SignInfos           = null,
                                      IEnumerable<Signature>?       Signatures          = null,

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

        #region TriggerMessageResponse(Request, Result)

        /// <summary>
        /// Create a new trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public TriggerMessageResponse(CS.TriggerMessageRequest  Request,
                                      Result                    Result)

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
        //       <ns:triggerMessageResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:triggerMessageResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:TriggerMessageResponse",
        //     "title":   "TriggerMessageResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
        //                 "NotImplemented"
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
        /// Parse the given XML representation of a trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static TriggerMessageResponse Parse(CS.TriggerMessageRequest  Request,
                                                   XElement                  XML)
        {

            if (TryParse(Request,
                         XML,
                         out var triggerMessageResponse,
                         out var errorResponse) &&
                triggerMessageResponse is not null)
            {
                return triggerMessageResponse;
            }

            throw new ArgumentException("The given XML representation of a trigger message response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomTriggerMessageResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomTriggerMessageResponseParser">An optional delegate to parse custom trigger message responses.</param>
        public static TriggerMessageResponse Parse(CS.TriggerMessageRequest                              Request,
                                                   JObject                                               JSON,
                                                   CustomJObjectParserDelegate<TriggerMessageResponse>?  CustomTriggerMessageResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var triggerMessageResponse,
                         out var errorResponse,
                         CustomTriggerMessageResponseParser) &&
                triggerMessageResponse is not null)
            {
                return triggerMessageResponse;
            }

            throw new ArgumentException("The given JSON representation of a trigger message response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out TriggerMessageResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="TriggerMessageResponse">The parsed trigger message response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.TriggerMessageRequest     Request,
                                       XElement                     XML,
                                       out TriggerMessageResponse?  TriggerMessageResponse,
                                       out String?                  ErrorResponse)
        {

            try
            {

                TriggerMessageResponse = new TriggerMessageResponse(

                                             Request,

                                             XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                TriggerMessageStatusExtensions.Parse)

                                         );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                TriggerMessageResponse  = null;
                ErrorResponse           = "The given XML representation of a trigger message response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out TriggerMessageResponse, out ErrorResponse, CustomTriggerMessageResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="TriggerMessageResponse">The parsed trigger message response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTriggerMessageResponseParser">An optional delegate to parse custom trigger message responses.</param>
        public static Boolean TryParse(CS.TriggerMessageRequest                              Request,
                                       JObject                                               JSON,
                                       out TriggerMessageResponse?                           TriggerMessageResponse,
                                       out String?                                           ErrorResponse,
                                       CustomJObjectParserDelegate<TriggerMessageResponse>?  CustomTriggerMessageResponseParser   = null)
        {

            try
            {

                TriggerMessageResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "trigger message status",
                                       TriggerMessageStatusExtensions.Parse,
                                       out TriggerMessageStatus Status,
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


                TriggerMessageResponse = new TriggerMessageResponse(

                                             Request,
                                             Status,
                                             null,

                                             null,
                                             null,
                                             Signatures,

                                             CustomData

                                         );

                if (CustomTriggerMessageResponseParser is not null)
                    TriggerMessageResponse = CustomTriggerMessageResponseParser(JSON,
                                                                                TriggerMessageResponse);

                return true;

            }
            catch (Exception e)
            {
                TriggerMessageResponse  = null;
                ErrorResponse           = "The given JSON representation of a trigger message response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "triggerMessageResponse",

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())

               );

        #endregion

        #region ToJSON(CustomTriggerMessageResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTriggerMessageResponseSerializer">A delegate to serialize custom trigger message responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TriggerMessageResponse>?  CustomTriggerMessageResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?          CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
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

            return CustomTriggerMessageResponseSerializer is not null
                       ? CustomTriggerMessageResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The trigger message command failed.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        public static TriggerMessageResponse Failed(CS.TriggerMessageRequest  Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (TriggerMessageResponse1, TriggerMessageResponse2)

        /// <summary>
        /// Compares two trigger message responses for equality.
        /// </summary>
        /// <param name="TriggerMessageResponse1">A trigger message response.</param>
        /// <param name="TriggerMessageResponse2">Another trigger message response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TriggerMessageResponse? TriggerMessageResponse1,
                                           TriggerMessageResponse? TriggerMessageResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TriggerMessageResponse1, TriggerMessageResponse2))
                return true;

            // If one is null, but not both, return false.
            if (TriggerMessageResponse1 is null || TriggerMessageResponse2 is null)
                return false;

            return TriggerMessageResponse1.Equals(TriggerMessageResponse2);

        }

        #endregion

        #region Operator != (TriggerMessageResponse1, TriggerMessageResponse2)

        /// <summary>
        /// Compares two trigger message responses for inequality.
        /// </summary>
        /// <param name="TriggerMessageResponse1">A trigger message response.</param>
        /// <param name="TriggerMessageResponse2">Another trigger message response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TriggerMessageResponse? TriggerMessageResponse1,
                                           TriggerMessageResponse? TriggerMessageResponse2)

            => !(TriggerMessageResponse1 == TriggerMessageResponse2);

        #endregion

        #endregion

        #region IEquatable<TriggerMessageResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two trigger message responses for equality.
        /// </summary>
        /// <param name="Object">A trigger message response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TriggerMessageResponse triggerMessageResponse &&
                   Equals(triggerMessageResponse);

        #endregion

        #region Equals(TriggerMessageResponse)

        /// <summary>
        /// Compares two trigger message responses for equality.
        /// </summary>
        /// <param name="TriggerMessageResponse">A trigger message response to compare with.</param>
        public override Boolean Equals(TriggerMessageResponse? TriggerMessageResponse)

            => TriggerMessageResponse is not null &&
                   Status.Equals(TriggerMessageResponse.Status);

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
