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
using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A send local list response.
    /// </summary>
    public class SendLocalListResponse : AResponse<CS.SendLocalListRequest,
                                                      SendLocalListResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the send local list command.
        /// </summary>
        public UpdateStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region SendLocalListResponse(Request, Status)

        /// <summary>
        /// Create a new send local list response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Status">The success or failure of the send local list command.</param>
        public SendLocalListResponse(CS.SendLocalListRequest  Request,
                                     UpdateStatus             Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region SendLocalListResponse(Request, Result)

        /// <summary>
        /// Create a new send local list response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
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

        #region (static) Parse   (Request, SendLocalListResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a send local list response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="SendLocalListResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SendLocalListResponse Parse(CS.SendLocalListRequest  Request,
                                                  XElement                 SendLocalListResponseXML,
                                                  OnExceptionDelegate      OnException = null)
        {

            if (TryParse(Request,
                         SendLocalListResponseXML,
                         out SendLocalListResponse sendLocalListResponse,
                         OnException))
            {
                return sendLocalListResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, SendLocalListResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a send local list response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="SendLocalListResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SendLocalListResponse Parse(CS.SendLocalListRequest  Request,
                                                  JObject                  SendLocalListResponseJSON,
                                                  OnExceptionDelegate      OnException = null)
        {

            if (TryParse(Request,
                         SendLocalListResponseJSON,
                         out SendLocalListResponse sendLocalListResponse,
                         OnException))
            {
                return sendLocalListResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, SendLocalListResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a send local list response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="SendLocalListResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SendLocalListResponse Parse(CS.SendLocalListRequest  Request,
                                                  String                   SendLocalListResponseText,
                                                  OnExceptionDelegate      OnException = null)
        {

            if (TryParse(Request,
                         SendLocalListResponseText,
                         out SendLocalListResponse sendLocalListResponse,
                         OnException))
            {
                return sendLocalListResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, SendLocalListResponseXML,  out SendLocalListResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a send local list response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="SendLocalListResponseXML">The XML to be parsed.</param>
        /// <param name="SendLocalListResponse">The parsed send local list response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.SendLocalListRequest    Request,
                                       XElement                   SendLocalListResponseXML,
                                       out SendLocalListResponse  SendLocalListResponse,
                                       OnExceptionDelegate        OnException  = null)
        {

            try
            {

                SendLocalListResponse = new SendLocalListResponse(

                                            Request,

                                            SendLocalListResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                    UpdateStatusExtentions.Parse)

                                        );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, SendLocalListResponseXML, e);

                SendLocalListResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, SendLocalListResponseJSON, out SendLocalListResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a send local list response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="SendLocalListResponseJSON">The JSON to be parsed.</param>
        /// <param name="SendLocalListResponse">The parsed send local list response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.SendLocalListRequest    Request,
                                       JObject                    SendLocalListResponseJSON,
                                       out SendLocalListResponse  SendLocalListResponse,
                                       OnExceptionDelegate        OnException  = null)
        {

            try
            {

                SendLocalListResponse = null;

                #region UpdateStatus

                if (!SendLocalListResponseJSON.MapMandatory("status",
                                                            "update status",
                                                            UpdateStatusExtentions.Parse,
                                                            out UpdateStatus  UpdateStatus,
                                                            out String        ErrorResponse))
                {
                    return false;
                }

                #endregion


                SendLocalListResponse = new SendLocalListResponse(Request,
                                                                  UpdateStatus);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, SendLocalListResponseJSON, e);

                SendLocalListResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, SendLocalListResponseText, out SendLocalListResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a send local list response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="SendLocalListResponseText">The text to be parsed.</param>
        /// <param name="SendLocalListResponse">The parsed send local list response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.SendLocalListRequest    Request,
                                       String                     SendLocalListResponseText,
                                       out SendLocalListResponse  SendLocalListResponse,
                                       OnExceptionDelegate        OnException  = null)
        {

            try
            {

                SendLocalListResponseText = SendLocalListResponseText?.Trim();

                if (SendLocalListResponseText.IsNotNullOrEmpty())
                {

                    if (SendLocalListResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(SendLocalListResponseText),
                                 out SendLocalListResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(SendLocalListResponseText).Root,
                                 out SendLocalListResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, SendLocalListResponseText, e);
            }

            SendLocalListResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "sendLocalListResponse",

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())

               );

        #endregion

        #region ToJSON(CustomSendLocalListResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSendLocalListResponseSerializer">A delegate to serialize custom send local list responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SendLocalListResponse>  CustomSendLocalListResponseSerializer   = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("status",  Status.ToString())
                       );

            return CustomSendLocalListResponseSerializer != null
                       ? CustomSendLocalListResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The send local list command failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static SendLocalListResponse Failed(CS.SendLocalListRequest  Request)

            => new SendLocalListResponse(Request,
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
        public static Boolean operator == (SendLocalListResponse SendLocalListResponse1, SendLocalListResponse SendLocalListResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SendLocalListResponse1, SendLocalListResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((SendLocalListResponse1 is null) || (SendLocalListResponse2 is null))
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
        public static Boolean operator != (SendLocalListResponse SendLocalListResponse1, SendLocalListResponse SendLocalListResponse2)

            => !(SendLocalListResponse1 == SendLocalListResponse2);

        #endregion

        #endregion

        #region IEquatable<SendLocalListResponse> Members

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

            if (!(Object is SendLocalListResponse SendLocalListResponse))
                return false;

            return Equals(SendLocalListResponse);

        }

        #endregion

        #region Equals(SendLocalListResponse)

        /// <summary>
        /// Compares two send local list responses for equality.
        /// </summary>
        /// <param name="SendLocalListResponse">A send local list response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(SendLocalListResponse SendLocalListResponse)
        {

            if (SendLocalListResponse is null)
                return false;

            return Status.Equals(SendLocalListResponse.Status);

        }

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
