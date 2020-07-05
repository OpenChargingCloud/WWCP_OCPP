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
    /// A unlock connector response.
    /// </summary>
    public class UnlockConnectorResponse : AResponse<CS.UnlockConnectorRequest,
                                                        UnlockConnectorResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the unlock connector command.
        /// </summary>
        public UnlockStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region UnlockConnectorResponse(Request, Status)

        /// <summary>
        /// Create a new unlock connector response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Status">The success or failure of the unlock connector command.</param>
        public UnlockConnectorResponse(CS.UnlockConnectorRequest  Request,
                                       UnlockStatus               Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region UnlockConnectorResponse(Result)

        /// <summary>
        /// Create a new unlock connector response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public UnlockConnectorResponse(CS.UnlockConnectorRequest  Request,
                                       Result                     Result)

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
        //       <ns:unlockConnectorResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:unlockConnectorResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:UnlockConnectorResponse",
        //     "title":   "UnlockConnectorResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Unlocked",
        //                 "UnlockFailed",
        //                 "NotSupported"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, UnlockConnectorResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a unlock connector response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="UnlockConnectorResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UnlockConnectorResponse Parse(CS.UnlockConnectorRequest  Request,
                                                    XElement                   UnlockConnectorResponseXML,
                                                    OnExceptionDelegate        OnException = null)
        {

            if (TryParse(Request,
                         UnlockConnectorResponseXML,
                         out UnlockConnectorResponse unlockConnectorResponse,
                         OnException))
            {
                return unlockConnectorResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, UnlockConnectorResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a unlock connector response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="UnlockConnectorResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UnlockConnectorResponse Parse(CS.UnlockConnectorRequest  Request,
                                                    JObject                    UnlockConnectorResponseJSON,
                                                    OnExceptionDelegate        OnException = null)
        {

            if (TryParse(Request,
                         UnlockConnectorResponseJSON,
                         out UnlockConnectorResponse unlockConnectorResponse,
                         OnException))
            {
                return unlockConnectorResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, UnlockConnectorResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a unlock connector response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="UnlockConnectorResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UnlockConnectorResponse Parse(CS.UnlockConnectorRequest  Request,
                                                    String                     UnlockConnectorResponseText,
                                                    OnExceptionDelegate        OnException = null)
        {

            if (TryParse(Request,
                         UnlockConnectorResponseText,
                         out UnlockConnectorResponse unlockConnectorResponse,
                         OnException))
            {
                return unlockConnectorResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, UnlockConnectorResponseXML,  out UnlockConnectorResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a unlock connector response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="UnlockConnectorResponseXML">The XML to be parsed.</param>
        /// <param name="UnlockConnectorResponse">The parsed unlock connector response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.UnlockConnectorRequest    Request,
                                       XElement                     UnlockConnectorResponseXML,
                                       out UnlockConnectorResponse  UnlockConnectorResponse,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                UnlockConnectorResponse = new UnlockConnectorResponse(

                                              Request,

                                              UnlockConnectorResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                        UnlockStatusExtentions.Parse)

                                          );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, UnlockConnectorResponseXML, e);

                UnlockConnectorResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, UnlockConnectorResponseJSON, out UnlockConnectorResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a unlock connector response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="UnlockConnectorResponseJSON">The JSON to be parsed.</param>
        /// <param name="UnlockConnectorResponse">The parsed unlock connector response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.UnlockConnectorRequest    Request,
                                       JObject                      UnlockConnectorResponseJSON,
                                       out UnlockConnectorResponse  UnlockConnectorResponse,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                UnlockConnectorResponse = null;

                #region UnlockStatus

                if (!UnlockConnectorResponseJSON.MapMandatory("status",
                                                              "unlock status",
                                                              UnlockStatusExtentions.Parse,
                                                              out UnlockStatus  UnlockStatus,
                                                              out String        ErrorResponse))
                {
                    return false;
                }

                #endregion


                UnlockConnectorResponse = new UnlockConnectorResponse(Request,
                                                                      UnlockStatus);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, UnlockConnectorResponseJSON, e);

                UnlockConnectorResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, UnlockConnectorResponseText, out UnlockConnectorResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a unlock connector response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="UnlockConnectorResponseText">The text to be parsed.</param>
        /// <param name="UnlockConnectorResponse">The parsed unlock connector response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.UnlockConnectorRequest    Request,
                                       String                       UnlockConnectorResponseText,
                                       out UnlockConnectorResponse  UnlockConnectorResponse,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                UnlockConnectorResponseText = UnlockConnectorResponseText?.Trim();

                if (UnlockConnectorResponseText.IsNotNullOrEmpty())
                {

                    if (UnlockConnectorResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(UnlockConnectorResponseText),
                                 out UnlockConnectorResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(UnlockConnectorResponseText).Root,
                                 out UnlockConnectorResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, UnlockConnectorResponseText, e);
            }

            UnlockConnectorResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "unlockConnectorResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomUnlockConnectorResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnlockConnectorResponseSerializer">A delegate to serialize custom unlock connector responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnlockConnectorResponse>  CustomUnlockConnectorResponseSerializer  = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomUnlockConnectorResponseSerializer != null
                       ? CustomUnlockConnectorResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The unlock connector command failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static UnlockConnectorResponse Failed(CS.UnlockConnectorRequest Request)

            => new UnlockConnectorResponse(Request,
                                           Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (UnlockConnectorResponse1, UnlockConnectorResponse2)

        /// <summary>
        /// Compares two unlock connector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse1">A unlock connector response.</param>
        /// <param name="UnlockConnectorResponse2">Another unlock connector response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnlockConnectorResponse UnlockConnectorResponse1, UnlockConnectorResponse UnlockConnectorResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnlockConnectorResponse1, UnlockConnectorResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((UnlockConnectorResponse1 is null) || (UnlockConnectorResponse2 is null))
                return false;

            return UnlockConnectorResponse1.Equals(UnlockConnectorResponse2);

        }

        #endregion

        #region Operator != (UnlockConnectorResponse1, UnlockConnectorResponse2)

        /// <summary>
        /// Compares two unlock connector responses for inequality.
        /// </summary>
        /// <param name="UnlockConnectorResponse1">A unlock connector response.</param>
        /// <param name="UnlockConnectorResponse2">Another unlock connector response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnlockConnectorResponse UnlockConnectorResponse1, UnlockConnectorResponse UnlockConnectorResponse2)

            => !(UnlockConnectorResponse1 == UnlockConnectorResponse2);

        #endregion

        #endregion

        #region IEquatable<UnlockConnectorResponse> Members

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

            if (!(Object is UnlockConnectorResponse UnlockConnectorResponse))
                return false;

            return Equals(UnlockConnectorResponse);

        }

        #endregion

        #region Equals(UnlockConnectorResponse)

        /// <summary>
        /// Compares two unlock connector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse">A unlock connector response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(UnlockConnectorResponse UnlockConnectorResponse)
        {

            if (UnlockConnectorResponse is null)
                return false;

            return Status.Equals(UnlockConnectorResponse.Status);

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
