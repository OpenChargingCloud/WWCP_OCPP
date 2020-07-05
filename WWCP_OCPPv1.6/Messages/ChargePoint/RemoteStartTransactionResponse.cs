/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// A remote start transaction response.
    /// </summary>
    public class RemoteStartTransactionResponse : AResponse<CS.RemoteStartTransactionRequest,
                                                               RemoteStartTransactionResponse>
    {

        #region Properties

        /// <summary>
        /// The status indicating whether the charge point accepts
        /// the request to start a charging transaction.
        /// </summary>
        public RemoteStartStopStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region RemoteStartTransactionResponse(Request, Status)

        /// <summary>
        /// Create a new remote start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Status">The status indicating whether the charge point accepts the request to start a charging transaction.</param>
        public RemoteStartTransactionResponse(CS.RemoteStartTransactionRequest  Request,
                                              RemoteStartStopStatus             Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region RemoteStartTransactionResponse(Request, Result)

        /// <summary>
        /// Create a new remote start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
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

        #region (static) Parse   (Request, RemoteStartTransactionResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="RemoteStartTransactionResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStartTransactionResponse Parse(CS.RemoteStartTransactionRequest  Request,
                                                           XElement                          RemoteStartTransactionResponseXML,
                                                           OnExceptionDelegate               OnException = null)
        {

            if (TryParse(Request,
                         RemoteStartTransactionResponseXML,
                         out RemoteStartTransactionResponse remoteStartTransactionResponse,
                         OnException))
            {
                return remoteStartTransactionResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, RemoteStartTransactionResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="RemoteStartTransactionResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStartTransactionResponse Parse(CS.RemoteStartTransactionRequest  Request,
                                                           JObject                           RemoteStartTransactionResponseJSON,
                                                           OnExceptionDelegate               OnException = null)
        {

            if (TryParse(Request,
                         RemoteStartTransactionResponseJSON,
                         out RemoteStartTransactionResponse remoteStartTransactionResponse,
                         OnException))
            {
                return remoteStartTransactionResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, RemoteStartTransactionResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="RemoteStartTransactionResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStartTransactionResponse Parse(CS.RemoteStartTransactionRequest  Request,
                                                           String                            RemoteStartTransactionResponseText,
                                                           OnExceptionDelegate               OnException = null)
        {

            if (TryParse(Request,
                         RemoteStartTransactionResponseText,
                         out RemoteStartTransactionResponse remoteStartTransactionResponse,
                         OnException))
            {
                return remoteStartTransactionResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, RemoteStartTransactionResponseXML,  out RemoteStartTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="RemoteStartTransactionResponseXML">The XML to be parsed.</param>
        /// <param name="RemoteStartTransactionResponse">The parsed remote start transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.RemoteStartTransactionRequest    Request,
                                       XElement                            RemoteStartTransactionResponseXML,
                                       out RemoteStartTransactionResponse  RemoteStartTransactionResponse,
                                       OnExceptionDelegate                 OnException  = null)
        {

            try
            {

                RemoteStartTransactionResponse = new RemoteStartTransactionResponse(

                                                     Request,

                                                     RemoteStartTransactionResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                                      RemoteStartStopStatusExtentions.Parse)

                                                 );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, RemoteStartTransactionResponseXML, e);

                RemoteStartTransactionResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, RemoteStartTransactionResponseJSON, out RemoteStartTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="RemoteStartTransactionResponseJSON">The JSON to be parsed.</param>
        /// <param name="RemoteStartTransactionResponse">The parsed remote start transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.RemoteStartTransactionRequest    Request,
                                       JObject                             RemoteStartTransactionResponseJSON,
                                       out RemoteStartTransactionResponse  RemoteStartTransactionResponse,
                                       OnExceptionDelegate                 OnException  = null)
        {

            try
            {

                RemoteStartTransactionResponse = null;

                #region RemoteStartStopStatus

                if (!RemoteStartTransactionResponseJSON.MapMandatory("status",
                                                                     "remote start stop status",
                                                                     RemoteStartStopStatusExtentions.Parse,
                                                                     out RemoteStartStopStatus  RemoteStartStopStatus,
                                                                     out String                 ErrorResponse))
                {
                    return false;
                }

                #endregion


                RemoteStartTransactionResponse = new RemoteStartTransactionResponse(Request,
                                                                                    RemoteStartStopStatus);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, RemoteStartTransactionResponseJSON, e);

                RemoteStartTransactionResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, RemoteStartTransactionResponseText, out RemoteStartTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a remote start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="RemoteStartTransactionResponseText">The text to be parsed.</param>
        /// <param name="RemoteStartTransactionResponse">The parsed remote start transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.RemoteStartTransactionRequest    Request,
                                       String                              RemoteStartTransactionResponseText,
                                       out RemoteStartTransactionResponse  RemoteStartTransactionResponse,
                                       OnExceptionDelegate                 OnException  = null)
        {

            try
            {

                RemoteStartTransactionResponseText = RemoteStartTransactionResponseText?.Trim();

                if (RemoteStartTransactionResponseText.IsNotNullOrEmpty())
                {

                    if (RemoteStartTransactionResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(RemoteStartTransactionResponseText),
                                 out RemoteStartTransactionResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(RemoteStartTransactionResponseText).Root,
                                 out RemoteStartTransactionResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, RemoteStartTransactionResponseText, e);
            }

            RemoteStartTransactionResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "remoteStartTransactionResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomRemoteStartTransactionResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoteStartTransactionResponseSerializer">A delegate to serialize custom remote start transaction responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoteStartTransactionResponse>  CustomRemoteStartTransactionResponseSerializer  = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomRemoteStartTransactionResponseSerializer != null
                       ? CustomRemoteStartTransactionResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The stop transaction failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static RemoteStartTransactionResponse Failed(CS.RemoteStartTransactionRequest  Request)

            => new RemoteStartTransactionResponse(Request,
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
        public static Boolean operator == (RemoteStartTransactionResponse RemoteStartTransactionResponse1, RemoteStartTransactionResponse RemoteStartTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RemoteStartTransactionResponse1, RemoteStartTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((RemoteStartTransactionResponse1 is null) || (RemoteStartTransactionResponse2 is null))
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
        public static Boolean operator != (RemoteStartTransactionResponse RemoteStartTransactionResponse1, RemoteStartTransactionResponse RemoteStartTransactionResponse2)

            => !(RemoteStartTransactionResponse1 == RemoteStartTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<RemoteStartTransactionResponse> Members

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

            if (!(Object is RemoteStartTransactionResponse RemoteStartTransactionResponse))
                return false;

            return Equals(RemoteStartTransactionResponse);

        }

        #endregion

        #region Equals(RemoteStartTransactionResponse)

        /// <summary>
        /// Compares two remote start transaction responses for equality.
        /// </summary>
        /// <param name="RemoteStartTransactionResponse">A remote start transaction response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(RemoteStartTransactionResponse RemoteStartTransactionResponse)
        {

            if (RemoteStartTransactionResponse is null)
                return false;

            return Status.Equals(RemoteStartTransactionResponse.Status);

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
