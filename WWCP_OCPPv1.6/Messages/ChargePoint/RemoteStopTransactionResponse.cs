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
    /// A remote stop transaction response.
    /// </summary>
    public class RemoteStopTransactionResponse : AResponse<CS.RemoteStopTransactionRequest,
                                                              RemoteStopTransactionResponse>
    {

        #region Properties

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
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Status">The status indicating whether the charge point accepts the request to stop the charging transaction.</param>
        public RemoteStopTransactionResponse(CS.RemoteStopTransactionRequest  Request,
                                             RemoteStartStopStatus            Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region RemoteStopTransactionResponse(Request, Result)

        /// <summary>
        /// Create a new remote stop transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
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

        #region (static) Parse   (Request, RemoteStopTransactionResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a remote stop transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="RemoteStopTransactionResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStopTransactionResponse Parse(CS.RemoteStopTransactionRequest  Request,
                                                          XElement                         RemoteStopTransactionResponseXML,
                                                          OnExceptionDelegate              OnException = null)
        {

            if (TryParse(Request,
                         RemoteStopTransactionResponseXML,
                         out RemoteStopTransactionResponse remoteStopTransactionResponse,
                         OnException))
            {
                return remoteStopTransactionResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, RemoteStopTransactionResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a remote stop transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="RemoteStopTransactionResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStopTransactionResponse Parse(CS.RemoteStopTransactionRequest  Request,
                                                          JObject                          RemoteStopTransactionResponseJSON,
                                                          OnExceptionDelegate              OnException = null)
        {

            if (TryParse(Request,
                         RemoteStopTransactionResponseJSON,
                         out RemoteStopTransactionResponse remoteStopTransactionResponse,
                         OnException))
            {
                return remoteStopTransactionResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, RemoteStopTransactionResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a remote stop transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="RemoteStopTransactionResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStopTransactionResponse Parse(CS.RemoteStopTransactionRequest  Request,
                                                          String                           RemoteStopTransactionResponseText,
                                                          OnExceptionDelegate              OnException = null)
        {

            if (TryParse(Request,
                         RemoteStopTransactionResponseText,
                         out RemoteStopTransactionResponse remoteStopTransactionResponse,
                         OnException))
            {
                return remoteStopTransactionResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, RemoteStopTransactionResponseXML,  out RemoteStopTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a remote stop transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="RemoteStopTransactionResponseXML">The XML to be parsed.</param>
        /// <param name="RemoteStopTransactionResponse">The parsed remote stop transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.RemoteStopTransactionRequest    Request,
                                       XElement                           RemoteStopTransactionResponseXML,
                                       out RemoteStopTransactionResponse  RemoteStopTransactionResponse,
                                       OnExceptionDelegate                OnException  = null)
        {

            try
            {

                RemoteStopTransactionResponse = new RemoteStopTransactionResponse(

                                                    Request,

                                                    RemoteStopTransactionResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                                    RemoteStartStopStatusExtentions.Parse)

                                                );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, RemoteStopTransactionResponseXML, e);

                RemoteStopTransactionResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, RemoteStopTransactionResponseJSON, out RemoteStopTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a remote stop transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="RemoteStopTransactionResponseJSON">The JSON to be parsed.</param>
        /// <param name="RemoteStopTransactionResponse">The parsed remote stop transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.RemoteStopTransactionRequest    Request,
                                       JObject                            RemoteStopTransactionResponseJSON,
                                       out RemoteStopTransactionResponse  RemoteStopTransactionResponse,
                                       OnExceptionDelegate                OnException  = null)
        {

            try
            {

                RemoteStopTransactionResponse = null;

                #region RemoteStartStopStatus

                if (!RemoteStopTransactionResponseJSON.MapMandatory("status",
                                                                    "remote start stop status",
                                                                    RemoteStartStopStatusExtentions.Parse,
                                                                    out RemoteStartStopStatus  RemoteStartStopStatus,
                                                                    out String                 ErrorResponse))
                {
                    return false;
                }

                #endregion


                RemoteStopTransactionResponse = new RemoteStopTransactionResponse(Request,
                                                                                  RemoteStartStopStatus);

                return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, RemoteStopTransactionResponseJSON, e);
            }

            RemoteStopTransactionResponse = null;
            return false;

        }

        #endregion

        #region (static) TryParse(Request, RemoteStopTransactionResponseText, out RemoteStopTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a remote stop transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="RemoteStopTransactionResponseText">The text to be parsed.</param>
        /// <param name="RemoteStopTransactionResponse">The parsed remote stop transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.RemoteStopTransactionRequest    Request,
                                       String                             RemoteStopTransactionResponseText,
                                       out RemoteStopTransactionResponse  RemoteStopTransactionResponse,
                                       OnExceptionDelegate                OnException  = null)
        {

            try
            {

                RemoteStopTransactionResponseText = RemoteStopTransactionResponseText?.Trim();

                if (RemoteStopTransactionResponseText.IsNotNullOrEmpty())
                {

                    if (RemoteStopTransactionResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(RemoteStopTransactionResponseText),
                                 out RemoteStopTransactionResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(RemoteStopTransactionResponseText).Root,
                                 out RemoteStopTransactionResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, RemoteStopTransactionResponseText, e);
            }

            RemoteStopTransactionResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "remoteStopTransactionResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomRemoteStopTransactionResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoteStopTransactionResponseSerializer">A delegate to serialize custom remote stop transaction responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoteStopTransactionResponse>  CustomRemoteStopTransactionResponseSerializer  = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomRemoteStopTransactionResponseSerializer is not null
                       ? CustomRemoteStopTransactionResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The stop transaction failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static RemoteStopTransactionResponse Failed(CS.RemoteStopTransactionRequest  Request)

            => new RemoteStopTransactionResponse(Request,
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
        public static Boolean operator == (RemoteStopTransactionResponse RemoteStopTransactionResponse1, RemoteStopTransactionResponse RemoteStopTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RemoteStopTransactionResponse1, RemoteStopTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((RemoteStopTransactionResponse1 is null) || (RemoteStopTransactionResponse2 is null))
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
        public static Boolean operator != (RemoteStopTransactionResponse RemoteStopTransactionResponse1, RemoteStopTransactionResponse RemoteStopTransactionResponse2)

            => !(RemoteStopTransactionResponse1 == RemoteStopTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<RemoteStopTransactionResponse> Members

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

            if (!(Object is RemoteStopTransactionResponse RemoteStopTransactionResponse))
                return false;

            return Equals(RemoteStopTransactionResponse);

        }

        #endregion

        #region Equals(RemoteStopTransactionResponse)

        /// <summary>
        /// Compares two remote stop transaction responses for equality.
        /// </summary>
        /// <param name="RemoteStopTransactionResponse">A remote stop transaction response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(RemoteStopTransactionResponse RemoteStopTransactionResponse)
        {

            if (RemoteStopTransactionResponse is null)
                return false;

            return Status.Equals(RemoteStopTransactionResponse.Status);

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
