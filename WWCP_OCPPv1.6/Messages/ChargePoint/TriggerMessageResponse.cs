/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A trigger message response.
    /// </summary>
    public class TriggerMessageResponse : AResponse<CS.TriggerMessageRequest,
                                                       TriggerMessageResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the trigger message command.
        /// </summary>
        public TriggerMessageStatus  Status   { get; }

        #endregion

        #region Constructor(s)

        #region TriggerMessageResponse(Request, Status)

        /// <summary>
        /// Create a new trigger message response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Status">The success or failure of the trigger message command.</param>
        public TriggerMessageResponse(CS.TriggerMessageRequest  Request,
                                      TriggerMessageStatus      Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region TriggerMessageResponse(Request, Result)

        /// <summary>
        /// Create a new trigger message response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
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

        #region (static) Parse   (Request, TriggerMessageResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a trigger message response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="TriggerMessageResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static TriggerMessageResponse Parse(CS.TriggerMessageRequest  Request,
                                                   XElement                  TriggerMessageResponseXML,
                                                   OnExceptionDelegate       OnException = null)
        {

            if (TryParse(Request,
                         TriggerMessageResponseXML,
                         out TriggerMessageResponse triggerMessageResponse,
                         OnException))
            {
                return triggerMessageResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, TriggerMessageResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a trigger message response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="TriggerMessageResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static TriggerMessageResponse Parse(CS.TriggerMessageRequest  Request,
                                                   JObject                   TriggerMessageResponseJSON,
                                                   OnExceptionDelegate       OnException = null)
        {

            if (TryParse(Request,
                         TriggerMessageResponseJSON,
                         out TriggerMessageResponse triggerMessageResponse,
                         OnException))
            {
                return triggerMessageResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, TriggerMessageResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a trigger message response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="TriggerMessageResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static TriggerMessageResponse Parse(CS.TriggerMessageRequest  Request,
                                                   String                    TriggerMessageResponseText,
                                                   OnExceptionDelegate       OnException = null)
        {

            if (TryParse(Request,
                         TriggerMessageResponseText,
                         out TriggerMessageResponse triggerMessageResponse,
                         OnException))
            {
                return triggerMessageResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, TriggerMessageResponseXML,  out TriggerMessageResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a trigger message response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="TriggerMessageResponseXML">The XML to be parsed.</param>
        /// <param name="TriggerMessageResponse">The parsed trigger message response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.TriggerMessageRequest    Request,
                                       XElement                    TriggerMessageResponseXML,
                                       out TriggerMessageResponse  TriggerMessageResponse,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                TriggerMessageResponse = new TriggerMessageResponse(

                                             Request,

                                             TriggerMessageResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                      TriggerMessageStatusExtentions.Parse)

                                         );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, TriggerMessageResponseXML, e);

                TriggerMessageResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, TriggerMessageResponseJSON, out TriggerMessageResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a trigger message response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="TriggerMessageResponseJSON">The JSON to be parsed.</param>
        /// <param name="TriggerMessageResponse">The parsed trigger message response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.TriggerMessageRequest    Request,
                                       JObject                     TriggerMessageResponseJSON,
                                       out TriggerMessageResponse  TriggerMessageResponse,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                TriggerMessageResponse = null;

                #region TriggerMessageStatus

                if (!TriggerMessageResponseJSON.MapMandatory("status",
                                                             "trigger message status",
                                                             TriggerMessageStatusExtentions.Parse,
                                                             out TriggerMessageStatus  TriggerMessageStatus,
                                                             out String                ErrorResponse))
                {
                    return false;
                }

                #endregion


                TriggerMessageResponse = new TriggerMessageResponse(Request,
                                                                    TriggerMessageStatus);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, TriggerMessageResponseJSON, e);

                TriggerMessageResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, TriggerMessageResponseText, out TriggerMessageResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a trigger message response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="TriggerMessageResponseText">The text to be parsed.</param>
        /// <param name="TriggerMessageResponse">The parsed trigger message response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.TriggerMessageRequest    Request,
                                       String                      TriggerMessageResponseText,
                                       out TriggerMessageResponse  TriggerMessageResponse,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                TriggerMessageResponseText = TriggerMessageResponseText?.Trim();

                if (TriggerMessageResponseText.IsNotNullOrEmpty())
                {

                    if (TriggerMessageResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(TriggerMessageResponseText),
                                 out TriggerMessageResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(TriggerMessageResponseText).Root,
                                 out TriggerMessageResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, TriggerMessageResponseText, e);
            }

            TriggerMessageResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "triggerMessageResponse",

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())

               );

        #endregion

        #region ToJSON(CustomTriggerMessageResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTriggerMessageResponseSerializer">A delegate to serialize custom trigger message responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TriggerMessageResponse>  CustomTriggerMessageResponseSerializer  = null)
        {

            var json = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
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
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static TriggerMessageResponse Failed(CS.TriggerMessageRequest  Request)

            => new TriggerMessageResponse(Request,
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
        public static Boolean operator == (TriggerMessageResponse TriggerMessageResponse1, TriggerMessageResponse TriggerMessageResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TriggerMessageResponse1, TriggerMessageResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((TriggerMessageResponse1 is null) || (TriggerMessageResponse2 is null))
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
        public static Boolean operator != (TriggerMessageResponse TriggerMessageResponse1, TriggerMessageResponse TriggerMessageResponse2)

            => !(TriggerMessageResponse1 == TriggerMessageResponse2);

        #endregion

        #endregion

        #region IEquatable<TriggerMessageResponse> Members

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

            if (!(Object is TriggerMessageResponse TriggerMessageResponse))
                return false;

            return Equals(TriggerMessageResponse);

        }

        #endregion

        #region Equals(TriggerMessageResponse)

        /// <summary>
        /// Compares two trigger message responses for equality.
        /// </summary>
        /// <param name="TriggerMessageResponse">A trigger message response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(TriggerMessageResponse TriggerMessageResponse)
        {

            if (TriggerMessageResponse is null)
                return false;

            return Status.Equals(TriggerMessageResponse.Status);

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
