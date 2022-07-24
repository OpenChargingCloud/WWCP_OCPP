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
    /// A clear charging profile response.
    /// </summary>
    public class ClearChargingProfileResponse : AResponse<CS.ClearChargingProfileRequest,
                                                             ClearChargingProfileResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the clear charging profile command.
        /// </summary>
        public ClearChargingProfileStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region ClearChargingProfileResponse(Request, Status)

        /// <summary>
        /// Create a new clear charging profile response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Status">The success or failure of the reset command.</param>
        public ClearChargingProfileResponse(CS.ClearChargingProfileRequest  Request,
                                            ClearChargingProfileStatus      Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region ClearChargingProfileResponse(Request, Result)

        /// <summary>
        /// Create a new clear charging profile response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ClearChargingProfileResponse(CS.ClearChargingProfileRequest  Request,
                                            Result                          Result)

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
        //       <ns:clearChargingProfileResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:clearChargingProfileResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (Request, ClearChargingProfileResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a clear charging profile response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ClearChargingProfileResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearChargingProfileResponse Parse(CS.ClearChargingProfileRequest  Request,
                                                         XElement                        ClearChargingProfileResponseXML,
                                                         OnExceptionDelegate             OnException = null)
        {

            if (TryParse(Request,
                         ClearChargingProfileResponseXML,
                         out ClearChargingProfileResponse clearChargingProfileResponse,
                         OnException))
            {
                return clearChargingProfileResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, ClearChargingProfileResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a clear charging profile response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ClearChargingProfileResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearChargingProfileResponse Parse(CS.ClearChargingProfileRequest  Request,
                                                         JObject                         ClearChargingProfileResponseJSON,
                                                         OnExceptionDelegate             OnException = null)
        {

            if (TryParse(Request,
                         ClearChargingProfileResponseJSON,
                         out ClearChargingProfileResponse clearChargingProfileResponse,
                         OnException))
            {
                return clearChargingProfileResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, ClearChargingProfileResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a clear charging profile response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ClearChargingProfileResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearChargingProfileResponse Parse(CS.ClearChargingProfileRequest  Request,
                                                         String                          ClearChargingProfileResponseText,
                                                         OnExceptionDelegate             OnException = null)
        {

            if (TryParse(Request,
                         ClearChargingProfileResponseText,
                         out ClearChargingProfileResponse clearChargingProfileResponse,
                         OnException))
            {
                return clearChargingProfileResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, ClearChargingProfileResponseXML,  out ClearChargingProfileResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a clear charging profile response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ClearChargingProfileResponseXML">The XML to be parsed.</param>
        /// <param name="ClearChargingProfileResponse">The parsed clear charging profile response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ClearChargingProfileRequest    Request,
                                       XElement                          ClearChargingProfileResponseXML,
                                       out ClearChargingProfileResponse  ClearChargingProfileResponse,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                ClearChargingProfileResponse = new ClearChargingProfileResponse(

                                                   Request,

                                                   ClearChargingProfileResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                                  ClearChargingProfileStatusExtentions.Parse)

                                               );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, ClearChargingProfileResponseXML, e);

                ClearChargingProfileResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, ClearChargingProfileResponseJSON, out ClearChargingProfileResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a clear charging profile response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ClearChargingProfileResponseJSON">The JSON to be parsed.</param>
        /// <param name="ClearChargingProfileResponse">The parsed clear charging profile response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ClearChargingProfileRequest    Request,
                                       JObject                           ClearChargingProfileResponseJSON,
                                       out ClearChargingProfileResponse  ClearChargingProfileResponse,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                ClearChargingProfileResponse = null;

                #region ClearChargingProfileStatus

                if (!ClearChargingProfileResponseJSON.MapMandatory("status",
                                                                   "clear charging profile status",
                                                                   ClearChargingProfileStatusExtentions.Parse,
                                                                   out ClearChargingProfileStatus  ClearChargingProfileStatus,
                                                                   out String                      ErrorResponse))
                {
                    return false;
                }

                #endregion


                ClearChargingProfileResponse = new ClearChargingProfileResponse(Request,
                                                                                ClearChargingProfileStatus);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, ClearChargingProfileResponseJSON, e);

                ClearChargingProfileResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, ClearChargingProfileResponseText, out ClearChargingProfileResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a clear charging profile response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ClearChargingProfileResponseText">The text to be parsed.</param>
        /// <param name="ClearChargingProfileResponse">The parsed clear charging profile response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ClearChargingProfileRequest    Request,
                                       String                            ClearChargingProfileResponseText,
                                       out ClearChargingProfileResponse  ClearChargingProfileResponse,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                ClearChargingProfileResponseText = ClearChargingProfileResponseText?.Trim();

                if (ClearChargingProfileResponseText.IsNotNullOrEmpty())
                {

                    if (ClearChargingProfileResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(ClearChargingProfileResponseText),
                                 out ClearChargingProfileResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(ClearChargingProfileResponseText).Root,
                                 out ClearChargingProfileResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, ClearChargingProfileResponseText, e);
            }

            ClearChargingProfileResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "clearChargingProfileResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomClearChargingProfileResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearChargingProfileResponseSerializer">A delegate to serialize custom clear charging profile responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearChargingProfileResponse>  CustomClearChargingProfileResponseSerializer  = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomClearChargingProfileResponseSerializer is not null
                       ? CustomClearChargingProfileResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The clear charging profile command failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static ClearChargingProfileResponse Failed(CS.ClearChargingProfileRequest Request)

            => new ClearChargingProfileResponse(Request,
                                                Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ClearChargingProfileResponse1, ClearChargingProfileResponse2)

        /// <summary>
        /// Compares two clear charging profile responses for equality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse1">A clear charging profile response.</param>
        /// <param name="ClearChargingProfileResponse2">Another clear charging profile response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearChargingProfileResponse ClearChargingProfileResponse1, ClearChargingProfileResponse ClearChargingProfileResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearChargingProfileResponse1, ClearChargingProfileResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((ClearChargingProfileResponse1 is null) || (ClearChargingProfileResponse2 is null))
                return false;

            return ClearChargingProfileResponse1.Equals(ClearChargingProfileResponse2);

        }

        #endregion

        #region Operator != (ClearChargingProfileResponse1, ClearChargingProfileResponse2)

        /// <summary>
        /// Compares two clear charging profile responses for inequality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse1">A clear charging profile response.</param>
        /// <param name="ClearChargingProfileResponse2">Another clear charging profile response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearChargingProfileResponse ClearChargingProfileResponse1, ClearChargingProfileResponse ClearChargingProfileResponse2)

            => !(ClearChargingProfileResponse1 == ClearChargingProfileResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfileResponse> Members

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

            if (!(Object is ClearChargingProfileResponse ClearChargingProfileResponse))
                return false;

            return Equals(ClearChargingProfileResponse);

        }

        #endregion

        #region Equals(ClearChargingProfileResponse)

        /// <summary>
        /// Compares two clear charging profile responses for equality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse">A clear charging profile response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ClearChargingProfileResponse ClearChargingProfileResponse)
        {

            if (ClearChargingProfileResponse is null)
                return false;

            return Status.Equals(ClearChargingProfileResponse.Status);

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
