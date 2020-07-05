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
    /// A reset response.
    /// </summary>
    public class ResetResponse : AResponse<CS.ResetRequest,
                                              ResetResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the reset command.
        /// </summary>
        public ResetStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region ResetResponse(Request, Status)

        /// <summary>
        /// Create a new reset response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Status">The success or failure of the reset command.</param>
        public ResetResponse(CS.ResetRequest  Request,
                             ResetStatus      Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region ResetResponse(Request, Result)

        /// <summary>
        /// Create a new reset response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ResetResponse(CS.ResetRequest  Request,
                             Result           Result)

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
        //       <ns:resetResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:resetResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ResetResponse",
        //     "title":   "ResetResponse",
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

        #region (static) Parse   (Request, ResetResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a reset response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ResetResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ResetResponse Parse(CS.ResetRequest      Request,
                                          XElement             ResetResponseXML,
                                          OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Request,
                         ResetResponseXML,
                         out ResetResponse resetResponse,
                         OnException))
            {
                return resetResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, ResetResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a reset response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ResetResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ResetResponse Parse(CS.ResetRequest      Request,
                                          JObject              ResetResponseJSON,
                                          OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Request,
                         ResetResponseJSON,
                         out ResetResponse resetResponse,
                         OnException))
            {
                return resetResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, ResetResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a reset response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ResetResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ResetResponse Parse(CS.ResetRequest      Request,
                                          String               ResetResponseText,
                                          OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Request,
                         ResetResponseText,
                         out ResetResponse resetResponse,
                         OnException))
            {
                return resetResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, ResetResponseXML,  out ResetResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a reset response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ResetResponseXML">The XML to be parsed.</param>
        /// <param name="ResetResponse">The parsed reset response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ResetRequest      Request,
                                       XElement             ResetResponseXML,
                                       out ResetResponse    ResetResponse,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                ResetResponse = new ResetResponse(

                                    Request,

                                    ResetResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                    ResetStatusExtentions.Parse)

                                );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ResetResponseXML, e);

                ResetResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, ResetResponseJSON, out ResetResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a reset response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ResetResponseJSON">The JSON to be parsed.</param>
        /// <param name="ResetResponse">The parsed reset response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ResetRequest      Request,
                                       JObject              ResetResponseJSON,
                                       out ResetResponse    ResetResponse,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                ResetResponse = null;

                #region ResetStatus

                if (!ResetResponseJSON.MapMandatory("status",
                                                    "reset status",
                                                    ResetStatusExtentions.Parse,
                                                    out ResetStatus  ResetStatus,
                                                    out String       ErrorResponse))
                {
                    return false;
                }

                #endregion


                ResetResponse = new ResetResponse(Request,
                                                  ResetStatus);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ResetResponseJSON, e);

                ResetResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, ResetResponseText, out ResetResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a reset response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ResetResponseText">The text to be parsed.</param>
        /// <param name="ResetResponse">The parsed reset response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ResetRequest      Request,
                                       String               ResetResponseText,
                                       out ResetResponse    ResetResponse,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                ResetResponseText = ResetResponseText?.Trim();

                if (ResetResponseText.IsNotNullOrEmpty())
                {

                    if (ResetResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(ResetResponseText),
                                 out ResetResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(ResetResponseText).Root,
                                 out ResetResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ResetResponseText, e);
            }

            ResetResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "resetResponse",

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())

               );

        #endregion

        #region ToJSON(CustomResetResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomResetResponseSerializer">A delegate to serialize custom reset responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ResetResponse>  CustomResetResponseSerializer  = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomResetResponseSerializer != null
                       ? CustomResetResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The reset command failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static ResetResponse Failed(CS.ResetRequest Request)

            => new ResetResponse(Request,
                                 Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ResetResponse1, ResetResponse2)

        /// <summary>
        /// Compares two reset responses for equality.
        /// </summary>
        /// <param name="ResetResponse1">A reset response.</param>
        /// <param name="ResetResponse2">Another reset response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ResetResponse ResetResponse1, ResetResponse ResetResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ResetResponse1, ResetResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((ResetResponse1 is null) || (ResetResponse2 is null))
                return false;

            return ResetResponse1.Equals(ResetResponse2);

        }

        #endregion

        #region Operator != (ResetResponse1, ResetResponse2)

        /// <summary>
        /// Compares two reset responses for inequality.
        /// </summary>
        /// <param name="ResetResponse1">A reset response.</param>
        /// <param name="ResetResponse2">Another reset response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ResetResponse ResetResponse1, ResetResponse ResetResponse2)

            => !(ResetResponse1 == ResetResponse2);

        #endregion

        #endregion

        #region IEquatable<ResetResponse> Members

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

            if (!(Object is ResetResponse ResetResponse))
                return false;

            return Equals(ResetResponse);

        }

        #endregion

        #region Equals(ResetResponse)

        /// <summary>
        /// Compares two reset responses for equality.
        /// </summary>
        /// <param name="ResetResponse">A reset response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ResetResponse ResetResponse)
        {

            if (ResetResponse is null)
                return false;

            return Status.Equals(ResetResponse.Status);

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
