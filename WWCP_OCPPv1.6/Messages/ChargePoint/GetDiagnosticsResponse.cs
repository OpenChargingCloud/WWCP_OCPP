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
    /// A get diagnostics response.
    /// </summary>
    public class GetDiagnosticsResponse : AResponse<CS.GetDiagnosticsRequest,
                                                       GetDiagnosticsResponse>
    {

        #region Data

        /// <summary>
        /// The maximum length of the name of the file with diagnostic information.
        /// </summary>
        public const UInt32  MaxFileNameLength  = 255;

        #endregion

        #region Properties

        /// <summary>
        /// The name of the file with diagnostic information that will
        /// be uploaded. This field is not present when no diagnostic
        /// information is available.
        /// </summary>
        public String  FileName    { get; }

        #endregion

        #region Constructor(s)

        #region GetDiagnosticsResponse(Request, Status)

        /// <summary>
        /// Create a new get diagnostics response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="FileName">The name of the file with diagnostic information that will be uploaded. This field is not present when no diagnostic information is available.</param>
        public GetDiagnosticsResponse(CS.GetDiagnosticsRequest  Request,
                                      String                    FileName = null)

            : base(Request,
                   Result.OK())

        {

            this.FileName = FileName;

        }

        #endregion

        #region GetDiagnosticsResponse(Request, Result)

        /// <summary>
        /// Create a new get diagnostics response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetDiagnosticsResponse(CS.GetDiagnosticsRequest  Request,
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
        //       <ns:getDiagnosticsResponse>
        //
        //          <ns:fileName>?</ns:fileName>
        //
        //       </ns:getDiagnosticsResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetDiagnosticsResponse",
        //     "title":   "GetDiagnosticsResponse",
        //     "type":    "object",
        //     "properties": {
        //         "fileName": {
        //             "type": "string",
        //             "maxLength": 255
        //         }
        //     },
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, GetDiagnosticsResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a get diagnostics response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetDiagnosticsResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetDiagnosticsResponse Parse(CS.GetDiagnosticsRequest  Request,
                                                   XElement                  GetDiagnosticsResponseXML,
                                                   OnExceptionDelegate       OnException = null)
        {

            if (TryParse(Request,
                         GetDiagnosticsResponseXML,
                         out GetDiagnosticsResponse getDiagnosticsResponse,
                         OnException))
            {
                return getDiagnosticsResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, GetDiagnosticsResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a get diagnostics response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetDiagnosticsResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetDiagnosticsResponse Parse(CS.GetDiagnosticsRequest  Request,
                                                   JObject                   GetDiagnosticsResponseJSON,
                                                   OnExceptionDelegate       OnException = null)
        {

            if (TryParse(Request,
                         GetDiagnosticsResponseJSON,
                         out GetDiagnosticsResponse getDiagnosticsResponse,
                         OnException))
            {
                return getDiagnosticsResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, GetDiagnosticsResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a get diagnostics response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetDiagnosticsResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetDiagnosticsResponse Parse(CS.GetDiagnosticsRequest  Request,
                                                   String                    GetDiagnosticsResponseText,
                                                   OnExceptionDelegate       OnException = null)
        {

            if (TryParse(Request,
                         GetDiagnosticsResponseText,
                         out GetDiagnosticsResponse getDiagnosticsResponse,
                         OnException))
            {
                return getDiagnosticsResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, GetDiagnosticsResponseXML,  out GetDiagnosticsResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a get diagnostics response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetDiagnosticsResponseXML">The XML to be parsed.</param>
        /// <param name="GetDiagnosticsResponse">The parsed get diagnostics response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.GetDiagnosticsRequest    Request,
                                       XElement                    GetDiagnosticsResponseXML,
                                       out GetDiagnosticsResponse  GetDiagnosticsResponse,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                GetDiagnosticsResponse = new GetDiagnosticsResponse(

                                             Request,

                                             GetDiagnosticsResponseXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CP + "fileName")

                                         );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, GetDiagnosticsResponseXML, e);

                GetDiagnosticsResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, GetDiagnosticsResponseJSON, out GetDiagnosticsResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get diagnostics response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetDiagnosticsResponseJSON">The JSON to be parsed.</param>
        /// <param name="GetDiagnosticsResponse">The parsed get diagnostics response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.GetDiagnosticsRequest    Request,
                                       JObject                     GetDiagnosticsResponseJSON,
                                       out GetDiagnosticsResponse  GetDiagnosticsResponse,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                GetDiagnosticsResponse = null;

                #region FileName

                var FileName = GetDiagnosticsResponseJSON.GetString("fileName");

                #endregion


                GetDiagnosticsResponse = new GetDiagnosticsResponse(Request,
                                                                    FileName);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, GetDiagnosticsResponseJSON, e);

                GetDiagnosticsResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, GetDiagnosticsResponseText, out GetDiagnosticsResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a get diagnostics response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetDiagnosticsResponseText">The text to be parsed.</param>
        /// <param name="GetDiagnosticsResponse">The parsed get diagnostics response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.GetDiagnosticsRequest    Request,
                                       String                      GetDiagnosticsResponseText,
                                       out GetDiagnosticsResponse  GetDiagnosticsResponse,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                GetDiagnosticsResponseText = GetDiagnosticsResponseText?.Trim();

                if (GetDiagnosticsResponseText.IsNotNullOrEmpty())
                {

                    if (GetDiagnosticsResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(GetDiagnosticsResponseText),
                                 out GetDiagnosticsResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(GetDiagnosticsResponseText).Root,
                                 out GetDiagnosticsResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, GetDiagnosticsResponseText, e);
            }

            GetDiagnosticsResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "getDiagnosticsResponse",

                   FileName != null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "fileName",  FileName.SubstringMax(MaxFileNameLength))
                       : null

               );

        #endregion

        #region ToJSON(CustomGetDiagnosticsResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetDiagnosticsResponseSerializer">A delegate to serialize custom get diagnostics responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetDiagnosticsResponse>  CustomGetDiagnosticsResponseSerializer  = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("fileName",  FileName.SubstringMax(MaxFileNameLength))
                       );

            return CustomGetDiagnosticsResponseSerializer != null
                       ? CustomGetDiagnosticsResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get diagnostics command failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static GetDiagnosticsResponse Failed(CS.GetDiagnosticsRequest  Request)

            => new GetDiagnosticsResponse(Request,
                                          Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetDiagnosticsResponse1, GetDiagnosticsResponse2)

        /// <summary>
        /// Compares two get diagnostics responses for equality.
        /// </summary>
        /// <param name="GetDiagnosticsResponse1">A get diagnostics response.</param>
        /// <param name="GetDiagnosticsResponse2">Another get diagnostics response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetDiagnosticsResponse GetDiagnosticsResponse1, GetDiagnosticsResponse GetDiagnosticsResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetDiagnosticsResponse1, GetDiagnosticsResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((GetDiagnosticsResponse1 is null) || (GetDiagnosticsResponse2 is null))
                return false;

            return GetDiagnosticsResponse1.Equals(GetDiagnosticsResponse2);

        }

        #endregion

        #region Operator != (GetDiagnosticsResponse1, GetDiagnosticsResponse2)

        /// <summary>
        /// Compares two get diagnostics responses for inequality.
        /// </summary>
        /// <param name="GetDiagnosticsResponse1">A get diagnostics response.</param>
        /// <param name="GetDiagnosticsResponse2">Another get diagnostics response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetDiagnosticsResponse GetDiagnosticsResponse1, GetDiagnosticsResponse GetDiagnosticsResponse2)

            => !(GetDiagnosticsResponse1 == GetDiagnosticsResponse2);

        #endregion

        #endregion

        #region IEquatable<GetDiagnosticsResponse> Members

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

            if (!(Object is GetDiagnosticsResponse GetDiagnosticsResponse))
                return false;

            return Equals(GetDiagnosticsResponse);

        }

        #endregion

        #region Equals(GetDiagnosticsResponse)

        /// <summary>
        /// Compares two get diagnostics responses for equality.
        /// </summary>
        /// <param name="GetDiagnosticsResponse">A get diagnostics response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(GetDiagnosticsResponse GetDiagnosticsResponse)
        {

            if (GetDiagnosticsResponse is null)
                return false;

            return (FileName == null && GetDiagnosticsResponse.FileName == null) ||
                   (FileName != null && GetDiagnosticsResponse.FileName != null && FileName.Equals(GetDiagnosticsResponse.FileName));

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => FileName != null
                   ? FileName.GetHashCode()
                   : base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => FileName ?? "GetDiagnosticsResponse";

        #endregion

    }

}
