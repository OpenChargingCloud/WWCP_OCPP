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
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A stop transaction response.
    /// </summary>
    public class StopTransactionResponse : AResponse<CP.StopTransactionRequest,
                                                        StopTransactionResponse>
    {

        #region Properties

        /// <summary>
        /// Information about authorization status, expiry and parent id.
        /// It is optional, because a transaction may have been stopped
        /// without an identifier.
        /// </summary>
        public IdTagInfo?  IdTagInfo   { get; }

        #endregion

        #region Constructor(s)

        #region StopTransactionResponse(Request, IdTagInfo)

        /// <summary>
        /// Create a new stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="IdTagInfo">Information about authorization status, expiry and parent id.</param>
        public StopTransactionResponse(CP.StopTransactionRequest  Request,
                                       IdTagInfo?                 IdTagInfo = null)

            : base(Request,
                   Result.OK())

        {

            this.IdTagInfo  = IdTagInfo;

        }

        #endregion

        #region StopTransactionResponse(Request, Result)

        /// <summary>
        /// Create a new stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public StopTransactionResponse(CP.StopTransactionRequest  Request,
                                       Result                     Result)

            : base(Request,
                   Result)

        {

            this.IdTagInfo = new IdTagInfo(AuthorizationStatus.Unknown);

        }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:stopTransactionResponse>
        //
        //          <!--Optional:-->
        //          <ns:idTagInfo>
        //
        //             <ns:status>?</ns:status>
        //
        //             <!--Optional:-->
        //             <ns:expiryDate>?</ns:expiryDate>
        //
        //             <!--Optional:-->
        //             <ns:parentIdTag>?</ns:parentIdTag>
        //
        //          </ns:idTagInfo>
        //
        //       </ns:stopTransactionResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:StopTransactionResponse",
        //     "title":    "StopTransactionResponse",
        //     "type":     "object",
        //     "properties": {
        //         "idTagInfo": {
        //             "type": "object",
        //             "properties": {
        //                 "expiryDate": {
        //                     "type":      "string",
        //                     "format":    "date-time"
        //                 },
        //                 "parentIdTag": {
        //                     "type":      "string",
        //                     "maxLength":  20
        //                 },
        //                 "status": {
        //                     "type":      "string",
        //                     "additionalProperties": false,
        //                     "enum": [
        //                         "Accepted",
        //                         "Blocked",
        //                         "Expired",
        //                         "Invalid",
        //                         "ConcurrentTx"
        //                     ]
        //                 }
        //             },
        //             "additionalProperties": false,
        //             "required": [
        //                 "status"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, StopTransactionResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="StopTransactionResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StopTransactionResponse Parse(CP.StopTransactionRequest  Request,
                                                    XElement                   StopTransactionResponseXML,
                                                    OnExceptionDelegate        OnException = null)
        {

            if (TryParse(Request,
                         StopTransactionResponseXML,
                         out StopTransactionResponse stopTransactionResponse,
                         OnException))
            {
                return stopTransactionResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, StopTransactionResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given text representation of a stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="StopTransactionResponseJSON">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StopTransactionResponse Parse(CP.StopTransactionRequest  Request,
                                                    JObject                    StopTransactionResponseJSON,
                                                    OnExceptionDelegate        OnException = null)
        {

            if (TryParse(Request,
                         StopTransactionResponseJSON,
                         out StopTransactionResponse stopTransactionResponse,
                         OnException))
            {
                return stopTransactionResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, StopTransactionResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="StopTransactionResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StopTransactionResponse Parse(CP.StopTransactionRequest  Request,
                                                    String                     StopTransactionResponseText,
                                                    OnExceptionDelegate        OnException = null)
        {

            if (TryParse(Request,
                         StopTransactionResponseText,
                         out StopTransactionResponse stopTransactionResponse,
                         OnException))
            {
                return stopTransactionResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, StopTransactionResponseXML,  out StopTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="StopTransactionResponseXML">The XML to be parsed.</param>
        /// <param name="StopTransactionResponse">The parsed stop transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.StopTransactionRequest    Request,
                                       XElement                     StopTransactionResponseXML,
                                       out StopTransactionResponse  StopTransactionResponse,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                StopTransactionResponse = new StopTransactionResponse(

                                              Request,

                                              StopTransactionResponseXML.MapElementOrNullable(OCPPNS.OCPPv1_6_CS + "idTagInfo",
                                                                                              OCPPv1_6.IdTagInfo.Parse)

                                          );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, StopTransactionResponseXML, e);

                StopTransactionResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, StopTransactionResponseJSON, out StopTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="JSON">The text to be parsed.</param>
        /// <param name="StopTransactionResponse">The parsed stop transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.StopTransactionRequest    Request,
                                       JObject                      JSON,
                                       out StopTransactionResponse  StopTransactionResponse,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                StopTransactionResponse = null;

                #region IdTagInfo

                if (!JSON.ParseMandatoryJSON("idTagInfo",
                                             "identification tag information",
                                             OCPPv1_6.IdTagInfo.TryParse,
                                             out IdTagInfo  IdTagInfo,
                                             out String     ErrorResponse))
                {
                    return false;
                }

                #endregion


                StopTransactionResponse = new StopTransactionResponse(Request,
                                                                      IdTagInfo);

                return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, JSON, e);
            }

            StopTransactionResponse = null;
            return false;

        }

        #endregion

        #region (static) TryParse(Request, StopTransactionResponseText, out StopTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a stop transaction response.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        /// <param name="StopTransactionResponseText">The text to be parsed.</param>
        /// <param name="StopTransactionResponse">The parsed stop transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.StopTransactionRequest    Request,
                                       String                       StopTransactionResponseText,
                                       out StopTransactionResponse  StopTransactionResponse,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                if (TryParse(Request,
                             XDocument.Parse(StopTransactionResponseText).Root,
                             out StopTransactionResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, StopTransactionResponseText, e);
            }

            StopTransactionResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "stopTransactionResponse",

                   IdTagInfo.HasValue
                       ? IdTagInfo.Value.ToXML()
                       : null

               );

        #endregion

        #region ToJSON(CustomStopTransactionResponseSerializer = null, CustomIdTagInfoResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStopTransactionResponseSerializer">A delegate to serialize custom start transaction responses.</param>
        /// <param name="CustomIdTagInfoResponseSerializer">A delegate to serialize custom IdTagInfos.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StopTransactionResponse>  CustomStopTransactionResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<IdTagInfo>                CustomIdTagInfoResponseSerializer         = null)
        {

            var JSON = JSONObject.Create(

                           IdTagInfo.HasValue
                               ? new JProperty("IdTagInfo",  IdTagInfo.Value.ToJSON(CustomIdTagInfoResponseSerializer))
                               : null

                       );

            return CustomStopTransactionResponseSerializer != null
                       ? CustomStopTransactionResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The stop transaction failed.
        /// </summary>
        /// <param name="Request">The stop transaction request leading to this response.</param>
        public static StopTransactionResponse Failed(CP.StopTransactionRequest Request)

            => new StopTransactionResponse(Request,
                                           Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (StopTransactionResponse1, StopTransactionResponse2)

        /// <summary>
        /// Compares two stop transaction responses for equality.
        /// </summary>
        /// <param name="StopTransactionResponse1">A stop transaction response.</param>
        /// <param name="StopTransactionResponse2">Another stop transaction response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StopTransactionResponse StopTransactionResponse1, StopTransactionResponse StopTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StopTransactionResponse1, StopTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((StopTransactionResponse1 is null) || (StopTransactionResponse2 is null))
                return false;

            return StopTransactionResponse1.Equals(StopTransactionResponse2);

        }

        #endregion

        #region Operator != (StopTransactionResponse1, StopTransactionResponse2)

        /// <summary>
        /// Compares two stop transaction responses for inequality.
        /// </summary>
        /// <param name="StopTransactionResponse1">A stop transaction response.</param>
        /// <param name="StopTransactionResponse2">Another stop transaction response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StopTransactionResponse StopTransactionResponse1, StopTransactionResponse StopTransactionResponse2)

            => !(StopTransactionResponse1 == StopTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<StopTransactionResponse> Members

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

            if (!(Object is StopTransactionResponse StopTransactionResponse))
                return false;

            return Equals(StopTransactionResponse);

        }

        #endregion

        #region Equals(StopTransactionResponse)

        /// <summary>
        /// Compares two stop transaction responses for equality.
        /// </summary>
        /// <param name="StopTransactionResponse">A stop transaction response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(StopTransactionResponse StopTransactionResponse)
        {

            if (StopTransactionResponse is null)
                return false;

            return IdTagInfo != null
                       ? IdTagInfo.Equals(StopTransactionResponse.IdTagInfo)
                       : Object.ReferenceEquals(this, StopTransactionResponse);

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

                return IdTagInfo != null
                           ? IdTagInfo.GetHashCode()
                           : base.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => IdTagInfo != null
                   ? IdTagInfo.ToString()
                   : "StopTransactionResponse";

        #endregion

    }

}
