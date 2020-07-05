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
    /// A change availability response.
    /// </summary>
    public class ChangeAvailabilityResponse : AResponse<CS.ChangeAvailabilityRequest,
                                                           ChangeAvailabilityResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the change availability command.
        /// </summary>
        public AvailabilityStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region ChangeAvailabilityResponse(Request, Status)

        /// <summary>
        /// Create a new change availability response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Status">The success or failure of the change availability command.</param>
        public ChangeAvailabilityResponse(CS.ChangeAvailabilityRequest  Request,
                                          AvailabilityStatus            Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region ChangeAvailabilityResponse(Request, Result)

        /// <summary>
        /// Create a new change availability response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ChangeAvailabilityResponse(CS.ChangeAvailabilityRequest  Request,
                                          Result                        Result)

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
        //       <ns:changeAvailabilityResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:changeAvailabilityResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ChangeAvailabilityResponse",
        //     "title":   "ChangeAvailabilityResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
        //                 "Scheduled"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, ChangeAvailabilityResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a change availability response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ChangeAvailabilityResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChangeAvailabilityResponse Parse(CS.ChangeAvailabilityRequest  Request,
                                                       XElement                      ChangeAvailabilityResponseXML,
                                                       OnExceptionDelegate           OnException = null)
        {

            if (TryParse(Request,
                         ChangeAvailabilityResponseXML,
                         out ChangeAvailabilityResponse changeAvailabilityResponse,
                         OnException))
            {
                return changeAvailabilityResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, ChangeAvailabilityResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a change availability response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ChangeAvailabilityResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChangeAvailabilityResponse Parse(CS.ChangeAvailabilityRequest  Request,
                                                       JObject                       ChangeAvailabilityResponseJSON,
                                                       OnExceptionDelegate           OnException = null)
        {

            if (TryParse(Request,
                         ChangeAvailabilityResponseJSON,
                         out ChangeAvailabilityResponse changeAvailabilityResponse,
                         OnException))
            {
                return changeAvailabilityResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, ChangeAvailabilityResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a change availability response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ChangeAvailabilityResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChangeAvailabilityResponse Parse(CS.ChangeAvailabilityRequest  Request,
                                                       String                        ChangeAvailabilityResponseText,
                                                       OnExceptionDelegate           OnException = null)
        {

            if (TryParse(Request,
                         ChangeAvailabilityResponseText,
                         out ChangeAvailabilityResponse changeAvailabilityResponse,
                         OnException))
            {
                return changeAvailabilityResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, ChangeAvailabilityResponseXML,  out ChangeAvailabilityResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a change availability response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ChangeAvailabilityResponseXML">The XML to be parsed.</param>
        /// <param name="ChangeAvailabilityResponse">The parsed change availability response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ChangeAvailabilityRequest    Request,
                                       XElement                        ChangeAvailabilityResponseXML,
                                       out ChangeAvailabilityResponse  ChangeAvailabilityResponse,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                ChangeAvailabilityResponse = new ChangeAvailabilityResponse(

                                                 Request,

                                                 ChangeAvailabilityResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                              AvailabilityStatusExtentions.Parse)

                                             );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ChangeAvailabilityResponseXML, e);

                ChangeAvailabilityResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, ChangeAvailabilityResponseJSON, out ChangeAvailabilityResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a change availability response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ChangeAvailabilityResponseJSON">The JSON to be parsed.</param>
        /// <param name="ChangeAvailabilityResponse">The parsed change availability response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ChangeAvailabilityRequest    Request,
                                       JObject                         ChangeAvailabilityResponseJSON,
                                       out ChangeAvailabilityResponse  ChangeAvailabilityResponse,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                ChangeAvailabilityResponse = null;

                #region AvailabilityStatus

                if (!ChangeAvailabilityResponseJSON.MapMandatory("status",
                                                                 "availability status",
                                                                 AvailabilityStatusExtentions.Parse,
                                                                 out AvailabilityStatus  AvailabilityStatus,
                                                                 out String              ErrorResponse))
                {
                    return false;
                }

                #endregion


                ChangeAvailabilityResponse = new ChangeAvailabilityResponse(Request,
                                                                            AvailabilityStatus);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ChangeAvailabilityResponseJSON, e);

                ChangeAvailabilityResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, ChangeAvailabilityResponseText, out ChangeAvailabilityResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a change availability response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ChangeAvailabilityResponseText">The text to be parsed.</param>
        /// <param name="ChangeAvailabilityResponse">The parsed change availability response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ChangeAvailabilityRequest    Request,
                                       String                          ChangeAvailabilityResponseText,
                                       out ChangeAvailabilityResponse  ChangeAvailabilityResponse,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                ChangeAvailabilityResponseText = ChangeAvailabilityResponseText?.Trim();

                if (ChangeAvailabilityResponseText.IsNotNullOrEmpty())
                {

                    if (ChangeAvailabilityResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(ChangeAvailabilityResponseText),
                                 out ChangeAvailabilityResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(ChangeAvailabilityResponseText).Root,
                                 out ChangeAvailabilityResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ChangeAvailabilityResponseText, e);
            }

            ChangeAvailabilityResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "changeAvailabilityResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomChangeAvailabilityResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChangeAvailabilityResponseSerializer">A delegate to serialize custom change availability responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChangeAvailabilityResponse>  CustomChangeAvailabilityResponseSerializer  = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomChangeAvailabilityResponseSerializer != null
                       ? CustomChangeAvailabilityResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The change availability command failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static ChangeAvailabilityResponse Failed(CS.ChangeAvailabilityRequest Request)

            => new ChangeAvailabilityResponse(Request,
                                              Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ChangeAvailabilityResponse1, ChangeAvailabilityResponse2)

        /// <summary>
        /// Compares two change availability responses for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityResponse1">A change availability response.</param>
        /// <param name="ChangeAvailabilityResponse2">Another change availability response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeAvailabilityResponse ChangeAvailabilityResponse1, ChangeAvailabilityResponse ChangeAvailabilityResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeAvailabilityResponse1, ChangeAvailabilityResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((ChangeAvailabilityResponse1 is null) || (ChangeAvailabilityResponse2 is null))
                return false;

            return ChangeAvailabilityResponse1.Equals(ChangeAvailabilityResponse2);

        }

        #endregion

        #region Operator != (ChangeAvailabilityResponse1, ChangeAvailabilityResponse2)

        /// <summary>
        /// Compares two change availability responses for inequality.
        /// </summary>
        /// <param name="ChangeAvailabilityResponse1">A change availability response.</param>
        /// <param name="ChangeAvailabilityResponse2">Another change availability response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeAvailabilityResponse ChangeAvailabilityResponse1, ChangeAvailabilityResponse ChangeAvailabilityResponse2)

            => !(ChangeAvailabilityResponse1 == ChangeAvailabilityResponse2);

        #endregion

        #endregion

        #region IEquatable<ChangeAvailabilityResponse> Members

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

            if (!(Object is ChangeAvailabilityResponse ChangeAvailabilityResponse))
                return false;

            return Equals(ChangeAvailabilityResponse);

        }

        #endregion

        #region Equals(ChangeAvailabilityResponse)

        /// <summary>
        /// Compares two change availability responses for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityResponse">A change availability response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ChangeAvailabilityResponse ChangeAvailabilityResponse)
        {

            if (ChangeAvailabilityResponse is null)
                return false;

            return Status.Equals(ChangeAvailabilityResponse.Status);

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
