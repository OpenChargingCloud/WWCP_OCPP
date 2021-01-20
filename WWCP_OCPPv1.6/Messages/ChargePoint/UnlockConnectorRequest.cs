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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A unlock connector request.
    /// </summary>
    public class UnlockConnectorRequest : ARequest<UnlockConnectorRequest>
    {

        #region Properties

        /// <summary>
        /// The identifier of the connector to be unlocked.
        /// </summary>
        public Connector_Id  ConnectorId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an unlock connector request.
        /// </summary>
        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
        public UnlockConnectorRequest(Connector_Id ConnectorId)
        {
            this.ConnectorId = ConnectorId;
        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //
        //    <soap:Header>
        //       ...
        //    </soap:Header>
        //
        //    <soap:Body>
        //       <ns:unlockConnectorRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //
        //       </ns:unlockConnectorRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:UnlockConnectorRequest",
        //     "title":   "UnlockConnectorRequest",
        //     "type":    "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "connectorId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (UnlockConnectorRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a unlock connector request.
        /// </summary>
        /// <param name="UnlockConnectorRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UnlockConnectorRequest Parse(XElement             UnlockConnectorRequestXML,
                                                   OnExceptionDelegate  OnException = null)
        {

            if (TryParse(UnlockConnectorRequestXML,
                         out UnlockConnectorRequest unlockConnectorRequest,
                         OnException))
            {
                return unlockConnectorRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (UnlockConnectorRequestJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a unlock connector request.
        /// </summary>
        /// <param name="UnlockConnectorRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UnlockConnectorRequest Parse(JObject              UnlockConnectorRequestJSON,
                                                   OnExceptionDelegate  OnException = null)
        {

            if (TryParse(UnlockConnectorRequestJSON,
                         out UnlockConnectorRequest unlockConnectorRequest,
                         OnException))
            {
                return unlockConnectorRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (UnlockConnectorRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a unlock connector request.
        /// </summary>
        /// <param name="UnlockConnectorRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UnlockConnectorRequest Parse(String               UnlockConnectorRequestText,
                                                   OnExceptionDelegate  OnException = null)
        {

            if (TryParse(UnlockConnectorRequestText,
                         out UnlockConnectorRequest unlockConnectorRequest,
                         OnException))
            {
                return unlockConnectorRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(UnlockConnectorRequestXML,  out UnlockConnectorRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a unlock connector request.
        /// </summary>
        /// <param name="UnlockConnectorRequestXML">The XML to be parsed.</param>
        /// <param name="UnlockConnectorRequest">The parsed unlock connector request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                    UnlockConnectorRequestXML,
                                       out UnlockConnectorRequest  UnlockConnectorRequest,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                UnlockConnectorRequest = new UnlockConnectorRequest(

                                             UnlockConnectorRequestXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                                      Connector_Id.Parse)

                                         );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, UnlockConnectorRequestXML, e);

                UnlockConnectorRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(UnlockConnectorRequestJSON,  out UnlockConnectorRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a unlock connector request.
        /// </summary>
        /// <param name="UnlockConnectorRequestJSON">The JSON to be parsed.</param>
        /// <param name="UnlockConnectorRequest">The parsed unlock connector request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                     UnlockConnectorRequestJSON,
                                       out UnlockConnectorRequest  UnlockConnectorRequest,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                UnlockConnectorRequest = null;

                #region ConnectorId

                if (!UnlockConnectorRequestJSON.ParseMandatory("connectorId",
                                                               "connector identification",
                                                               Connector_Id.TryParse,
                                                               out Connector_Id  ConnectorId,
                                                               out String        ErrorResponse))
                {
                    return false;
                }

                #endregion


                UnlockConnectorRequest = new UnlockConnectorRequest(ConnectorId);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, UnlockConnectorRequestJSON, e);

                UnlockConnectorRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(UnlockConnectorRequestText, out UnlockConnectorRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a unlock connector request.
        /// </summary>
        /// <param name="UnlockConnectorRequestText">The text to be parsed.</param>
        /// <param name="UnlockConnectorRequest">The parsed unlock connector request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                      UnlockConnectorRequestText,
                                       out UnlockConnectorRequest  UnlockConnectorRequest,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                UnlockConnectorRequestText = UnlockConnectorRequestText?.Trim();

                if (UnlockConnectorRequestText.IsNotNullOrEmpty())
                {

                    if (UnlockConnectorRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(UnlockConnectorRequestText),
                                 out UnlockConnectorRequest,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(UnlockConnectorRequestText).Root,
                                 out UnlockConnectorRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, UnlockConnectorRequestText, e);
            }

            UnlockConnectorRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "unlockConnectorRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",  ConnectorId.ToString())

               );

        #endregion

        #region ToJSON(CustomUnlockConnectorRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnlockConnectorRequestSerializer">A delegate to serialize custom unlock connector requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnlockConnectorRequest> CustomUnlockConnectorRequestSerializer  = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("connectorId",  ConnectorId.ToString())
                       );

            return CustomUnlockConnectorRequestSerializer != null
                       ? CustomUnlockConnectorRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two unlock connector requests for equality.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">A unlock connector request.</param>
        /// <param name="UnlockConnectorRequest2">Another unlock connector request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnlockConnectorRequest UnlockConnectorRequest1, UnlockConnectorRequest UnlockConnectorRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnlockConnectorRequest1, UnlockConnectorRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((UnlockConnectorRequest1 is null) || (UnlockConnectorRequest2 is null))
                return false;

            return UnlockConnectorRequest1.Equals(UnlockConnectorRequest2);

        }

        #endregion

        #region Operator != (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two unlock connector requests for inequality.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">A unlock connector request.</param>
        /// <param name="UnlockConnectorRequest2">Another unlock connector request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnlockConnectorRequest UnlockConnectorRequest1, UnlockConnectorRequest UnlockConnectorRequest2)

            => !(UnlockConnectorRequest1 == UnlockConnectorRequest2);

        #endregion

        #endregion

        #region IEquatable<UnlockConnectorRequest> Members

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

            if (!(Object is UnlockConnectorRequest UnlockConnectorRequest))
                return false;

            return Equals(UnlockConnectorRequest);

        }

        #endregion

        #region Equals(UnlockConnectorRequest)

        /// <summary>
        /// Compares two unlock connector requests for equality.
        /// </summary>
        /// <param name="UnlockConnectorRequest">A unlock connector request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(UnlockConnectorRequest UnlockConnectorRequest)
        {

            if (UnlockConnectorRequest is null)
                return false;

            return ConnectorId.Equals(UnlockConnectorRequest.ConnectorId);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => ConnectorId.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => ConnectorId.ToString();

        #endregion

    }

}
