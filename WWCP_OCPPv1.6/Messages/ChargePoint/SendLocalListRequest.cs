/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The SendLocalList request.
    /// </summary>
    public class SendLocalListRequest : ARequest<SendLocalListRequest>
    {

        #region Properties

        /// <summary>
        /// In case of a full update this is the version number of the
        /// full list. In case of a differential update it is the
        /// version number of the list after the update has been applied.
        /// </summary>
        public UInt64                          ListVersion               { get; }

        /// <summary>
        /// The type of update (full or differential).
        /// </summary>
        public UpdateTypes                     UpdateType                { get; }

        /// <summary>
        /// In case of a full update this contains the list of values that
        /// form the new local authorization list.
        /// In case of a differential update it contains the changes to be
        /// applied to the local authorization list in the charge point.
        /// Maximum number of AuthorizationData elements is available in
        /// the configuration key: SendLocalListMaxLength.
        /// </summary>
        public IEnumerable<AuthorizationData>  LocalAuthorizationList    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a SendLocalList request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ListVersion">In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.</param>
        /// <param name="UpdateType">The type of update (full or differential).</param>
        /// <param name="LocalAuthorizationList">In case of a full update this contains the list of values that form the new local authorization list. In case of a differential update it contains the changes to be applied to the local authorization list in the charge point. Maximum number of AuthorizationData elements is available in the configuration key: SendLocalListMaxLength.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public SendLocalListRequest(ChargeBox_Id                    ChargeBoxId,
                                    UInt64                          ListVersion,
                                    UpdateTypes                     UpdateType,
                                    IEnumerable<AuthorizationData>  LocalAuthorizationList   = null,

                                    Request_Id?                     RequestId                = null,
                                    DateTime?                       RequestTimestamp         = null,
                                    EventTracking_Id                EventTrackingId          = null)

            : base(ChargeBoxId,
                   "SendLocalList",
                   RequestId,
                   EventTrackingId,
                   RequestTimestamp)

        {

            this.ListVersion             = ListVersion;
            this.UpdateType              = UpdateType;
            this.LocalAuthorizationList  = LocalAuthorizationList ?? new AuthorizationData[0];

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //
        //    <soap:Header>
        //       ...
        //    </soap:Header>
        //
        //    <soap:Body>
        //       <ns:sendLocalListRequest>
        //
        //          <ns:listVersion>?</ns:listVersion>
        //
        //          <!--Zero or more repetitions:-->
        //          <ns:localAuthorizationList>
        //
        //             <ns:idTag>?</ns:idTag>
        //
        //             <!--Optional:-->
        //             <ns:idTagInfo>
        //
        //                <ns:status>?</ns:status>
        //
        //                <!--Optional:-->
        //                <ns:expiryDate>?</ns:expiryDate>
        //
        //                <!--Optional:-->
        //                <ns:parentIdTag>?</ns:parentIdTag>
        //
        //             </ns:idTagInfo>
        //
        //          </ns:localAuthorizationList>
        //
        //          <ns:updateType>?</ns:updateType>
        //
        //       </ns:sendLocalListRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:SendLocalListRequest",
        //     "title":   "SendLocalListRequest",
        //     "type":    "object",
        //     "properties": {
        //         "listVersion": {
        //             "type": "integer"
        //         },
        //         "localAuthorizationList": {
        //             "type": "array",
        //             "items": {
        //                 "type": "object",
        //                 "properties": {
        //                     "idTag": {
        //                         "type": "string",
        //                         "maxLength": 20
        //                     },
        //                     "idTagInfo": {
        //                         "type": "object",
        //                         "properties": {
        //                             "expiryDate": {
        //                                 "type": "string",
        //                                 "format": "date-time"
        //                             },
        //                             "parentIdTag": {
        //                                 "type": "string",
        //                                 "maxLength": 20
        //                             },
        //                             "status": {
        //                                 "type": "string",
        //                                 "additionalProperties": false,
        //                                 "enum": [
        //                                     "Accepted",
        //                                     "Blocked",
        //                                     "Expired",
        //                                     "Invalid",
        //                                     "ConcurrentTx"
        //                                 ]
        //                             }
        //                         },
        //                         "additionalProperties": false,
        //                         "required": [
        //                             "status"
        //                         ]
        //                     }
        //                 },
        //                 "additionalProperties": false,
        //                 "required": [
        //                     "idTag"
        //                 ]
        //             }
        //         },
        //         "updateType": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Differential",
        //                 "Full"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "listVersion",
        //         "updateType"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of a SendLocalList request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SendLocalListRequest Parse(XElement             XML,
                                                 Request_Id           RequestId,
                                                 ChargeBox_Id         ChargeBoxId,
                                                 OnExceptionDelegate  OnException = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out SendLocalListRequest sendLocalListRequest,
                         OnException))
            {
                return sendLocalListRequest;
            }

            throw new ArgumentException("The given XML representation of a SendLocalList request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomSendLocalListRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SendLocalList request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomSendLocalListRequestParser">A delegate to parse custom SendLocalList requests.</param>
        public static SendLocalListRequest Parse(JObject                                            JSON,
                                                 Request_Id                                         RequestId,
                                                 ChargeBox_Id                                       ChargeBoxId,
                                                 CustomJObjectParserDelegate<SendLocalListRequest>  CustomSendLocalListRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out SendLocalListRequest  sendLocalListRequest,
                         out String                ErrorResponse,
                         CustomSendLocalListRequestParser))
            {
                return sendLocalListRequest;
            }

            throw new ArgumentException("The given JSON representation of a SendLocalList request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of a SendLocalList request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SendLocalListRequest Parse(String               Text,
                                                 Request_Id           RequestId,
                                                 ChargeBox_Id         ChargeBoxId,
                                                 OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out SendLocalListRequest sendLocalListRequest,
                         OnException))
            {
                return sendLocalListRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out SendLocalListRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a SendLocalList request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SendLocalListRequest">The parsed SendLocalList request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                  XML,
                                       Request_Id                RequestId,
                                       ChargeBox_Id              ChargeBoxId,
                                       out SendLocalListRequest  SendLocalListRequest,
                                       OnExceptionDelegate       OnException  = null)
        {

            try
            {

                SendLocalListRequest = new SendLocalListRequest(

                                           ChargeBoxId,

                                           XML.MapValueOrFail     (OCPPNS.OCPPv1_6_CP + "listVersion",
                                                                                       UInt64.Parse),

                                           XML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "updateType",
                                                                                       UpdateTypesExtentions.Parse),

                                           XML.MapElements        (OCPPNS.OCPPv1_6_CP + "localAuthorizationList",
                                                                                       AuthorizationData.Parse),

                                           RequestId

                                       );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, XML, e);

                SendLocalListRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out SendLocalListRequest, out ErrorResponse, CustomSendLocalListRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a SendLocalList request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SendLocalListRequest">The parsed SendLocalList request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       Request_Id                                         RequestId,
                                       ChargeBox_Id                                       ChargeBoxId,
                                       out SendLocalListRequest                           SendLocalListRequest,
                                       out String                                         ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out SendLocalListRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a SendLocalList request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SendLocalListRequest">The parsed SendLocalList request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSendLocalListRequestParser">A delegate to parse custom SendLocalList requests.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       Request_Id                                         RequestId,
                                       ChargeBox_Id                                       ChargeBoxId,
                                       out SendLocalListRequest                           SendLocalListRequest,
                                       out String                                         ErrorResponse,
                                       CustomJObjectParserDelegate<SendLocalListRequest>  CustomSendLocalListRequestParser)
        {

            try
            {

                SendLocalListRequest = null;

                #region ListVersion               [mandatory]

                if (!JSON.ParseMandatory("listVersion",
                                         "list version",
                                         out UInt64 ListVersion,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region UpdateType                [mandatory]

                if (!JSON.MapMandatory("updateType",
                                       "update type",
                                       UpdateTypesExtentions.Parse,
                                       out UpdateTypes UpdateType,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region LocalAuthorizationList    [mandatory]

                if (!JSON.ParseMandatoryJSON("localAuthorizationList",
                                             "local authorization list",
                                             AuthorizationData.TryParse,
                                             out IEnumerable<AuthorizationData> LocalAuthorizationList,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargeBoxId               [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                SendLocalListRequest = new SendLocalListRequest(ChargeBoxId,
                                                                ListVersion,
                                                                UpdateType,
                                                                LocalAuthorizationList,
                                                                RequestId);

                if (CustomSendLocalListRequestParser != null)
                    SendLocalListRequest = CustomSendLocalListRequestParser(JSON,
                                                                            SendLocalListRequest);

                return true;

            }
            catch (Exception e)
            {
                SendLocalListRequest  = default;
                ErrorResponse         = "The given JSON representation of a SendLocalList request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(SendLocalListRequestText, RequestId, ChargeBoxId, out SendLocalListRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a SendLocalList request.
        /// </summary>
        /// <param name="SendLocalListRequestText">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SendLocalListRequest">The parsed SendLocalList request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                    SendLocalListRequestText,
                                       Request_Id                RequestId,
                                       ChargeBox_Id              ChargeBoxId,
                                       out SendLocalListRequest  SendLocalListRequest,
                                       OnExceptionDelegate       OnException  = null)
        {

            try
            {

                SendLocalListRequestText = SendLocalListRequestText?.Trim();

                if (SendLocalListRequestText.IsNotNullOrEmpty())
                {

                    if (SendLocalListRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(SendLocalListRequestText),
                                 RequestId,
                                 ChargeBoxId,
                                 out SendLocalListRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(SendLocalListRequestText).Root,
                                 RequestId,
                                 ChargeBoxId,
                                 out SendLocalListRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, SendLocalListRequestText, e);
            }

            SendLocalListRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "sendLocalListRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "listVersion",  ListVersion),

                   LocalAuthorizationList.IsNeitherNullNorEmpty()
                       ? LocalAuthorizationList.Select(item => item.ToXML(OCPPNS.OCPPv1_6_CP + "localAuthorizationList"))
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CP + "updateType",  UpdateType.AsText())

               );

        #endregion

        #region ToJSON(CustomSendLocalListRequestSerializer = null, CustomAuthorizationDataSerializer = null, CustomIdTagInfoResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public override JObject ToJSON()
            => ToJSON(null, null, null);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSendLocalListRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomAuthorizationDataSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomIdTagInfoResponseSerializer">A delegate to serialize custom IdTagInfos.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SendLocalListRequest> CustomSendLocalListRequestSerializer,
                              CustomJObjectSerializerDelegate<AuthorizationData>    CustomAuthorizationDataSerializer     = null,
                              CustomJObjectSerializerDelegate<IdTagInfo>            CustomIdTagInfoResponseSerializer     = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("listVersion",                   ListVersion),
                           new JProperty("updateType",                    UpdateType.AsText()),

                           LocalAuthorizationList.IsNeitherNullNorEmpty()
                               ? new JProperty("localAuthorizationList",  new JArray(LocalAuthorizationList.Select(item => item.ToJSON(CustomAuthorizationDataSerializer,
                                                                                                                                       CustomIdTagInfoResponseSerializer))))
                               : null

                       );

            return CustomSendLocalListRequestSerializer != null
                       ? CustomSendLocalListRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SendLocalListRequest1, SendLocalListRequest2)

        /// <summary>
        /// Compares two SendLocalList requests for equality.
        /// </summary>
        /// <param name="SendLocalListRequest1">A SendLocalList request.</param>
        /// <param name="SendLocalListRequest2">Another SendLocalList request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SendLocalListRequest SendLocalListRequest1, SendLocalListRequest SendLocalListRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SendLocalListRequest1, SendLocalListRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((SendLocalListRequest1 is null) || (SendLocalListRequest2 is null))
                return false;

            return SendLocalListRequest1.Equals(SendLocalListRequest2);

        }

        #endregion

        #region Operator != (SendLocalListRequest1, SendLocalListRequest2)

        /// <summary>
        /// Compares two SendLocalList requests for inequality.
        /// </summary>
        /// <param name="SendLocalListRequest1">A SendLocalList request.</param>
        /// <param name="SendLocalListRequest2">Another SendLocalList request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SendLocalListRequest SendLocalListRequest1, SendLocalListRequest SendLocalListRequest2)

            => !(SendLocalListRequest1 == SendLocalListRequest2);

        #endregion

        #endregion

        #region IEquatable<SendLocalListRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is SendLocalListRequest SendLocalListRequest))
                return false;

            return Equals(SendLocalListRequest);

        }

        #endregion

        #region Equals(SendLocalListRequest)

        /// <summary>
        /// Compares two SendLocalList requests for equality.
        /// </summary>
        /// <param name="SendLocalListRequest">A SendLocalList request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(SendLocalListRequest SendLocalListRequest)
        {

            if (SendLocalListRequest is null)
                return false;

            return ListVersion.                   Equals(SendLocalListRequest.ListVersion) &&
                   UpdateType.                    Equals(SendLocalListRequest.UpdateType)  &&
                   LocalAuthorizationList.Count().Equals(SendLocalListRequest.LocalAuthorizationList.Count());

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return ListVersion.           GetHashCode() * 5 ^
                       UpdateType.            GetHashCode() * 3 ^
                       LocalAuthorizationList.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(UpdateType.ToString(),
                             " of ",
                             ListVersion,
                             LocalAuthorizationList.Count(), " authorization list entries");

        #endregion

    }

}
