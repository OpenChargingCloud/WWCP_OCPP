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

namespace cloud.charging.adapters.OCPPv1_6.CS
{

    /// <summary>
    /// A change availability request.
    /// </summary>
    public class ChangeAvailabilityRequest : ARequest<ChangeAvailabilityRequest>
    {

        #region Properties

        /// <summary>
        /// The identification of the connector for which its availability
        /// should be changed. Id '0' (zero) is used if the availability of
        /// the entire charge point and all its connectors should be changed.
        /// </summary>
        public Connector_Id       ConnectorId    { get; }

        /// <summary>
        /// The new availability of the charge point or charge point connector.
        /// </summary>
        public AvailabilityTypes  Type           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a change availability request.
        /// </summary>
        /// <param name="ConnectorId">The identification of the connector for which its availability should be changed. Id '0' (zero) is used if the availability of the entire charge point and all its connectors should be changed.</param>
        /// <param name="Type">The new availability of the charge point or charge point connector.</param>
        public ChangeAvailabilityRequest(Connector_Id       ConnectorId,
                                         AvailabilityTypes  Type)
        {

            this.ConnectorId  = ConnectorId;
            this.Type         = Type;

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
        //       <ns:changeAvailabilityRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //          <ns:type>?</ns:type>
        //
        //       </ns:changeAvailabilityRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ChangeAvailabilityRequest",
        //     "title":   "ChangeAvailabilityRequest",
        //     "type":    "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //         "type": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Inoperative",
        //                 "Operative"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "connectorId",
        //         "type"
        //     ]
        // }

        #endregion

        #region (static) Parse   (ChangeAvailabilityRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a change availability request.
        /// </summary>
        /// <param name="ChangeAvailabilityRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChangeAvailabilityRequest Parse(XElement             ChangeAvailabilityRequestXML,
                                                      OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ChangeAvailabilityRequestXML,
                         out ChangeAvailabilityRequest changeAvailabilityRequest,
                         OnException))
            {
                return changeAvailabilityRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (ChangeAvailabilityRequestJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a change availability request.
        /// </summary>
        /// <param name="ChangeAvailabilityRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChangeAvailabilityRequest Parse(JObject              ChangeAvailabilityRequestJSON,
                                                      OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ChangeAvailabilityRequestJSON,
                         out ChangeAvailabilityRequest changeAvailabilityRequest,
                         OnException))
            {
                return changeAvailabilityRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (ChangeAvailabilityRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a change availability request.
        /// </summary>
        /// <param name="ChangeAvailabilityRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChangeAvailabilityRequest Parse(String               ChangeAvailabilityRequestText,
                                                      OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ChangeAvailabilityRequestText,
                         out ChangeAvailabilityRequest changeAvailabilityRequest,
                         OnException))
            {
                return changeAvailabilityRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(ChangeAvailabilityRequestXML,  out ChangeAvailabilityRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a change availability request.
        /// </summary>
        /// <param name="ChangeAvailabilityRequestXML">The XML to be parsed.</param>
        /// <param name="ChangeAvailabilityRequest">The parsed change availability request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                       ChangeAvailabilityRequestXML,
                                       out ChangeAvailabilityRequest  ChangeAvailabilityRequest,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                ChangeAvailabilityRequest = new ChangeAvailabilityRequest(

                                                ChangeAvailabilityRequestXML.MapValueOrFail     (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                                                 Connector_Id.Parse),

                                                ChangeAvailabilityRequestXML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "type",
                                                                                                 AvailabilityTypesExtentions.Parse)

                                            );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ChangeAvailabilityRequestXML, e);

                ChangeAvailabilityRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ChangeAvailabilityRequestJSON, out ChangeAvailabilityRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a change availability request.
        /// </summary>
        /// <param name="ChangeAvailabilityRequestJSON">The JSON to be parsed.</param>
        /// <param name="ChangeAvailabilityRequest">The parsed change availability request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                        ChangeAvailabilityRequestJSON,
                                       out ChangeAvailabilityRequest  ChangeAvailabilityRequest,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                ChangeAvailabilityRequest = null;

                #region ConnectorId

                if (!ChangeAvailabilityRequestJSON.ParseMandatory("connectorId",
                                                                  "connector identification",
                                                                  Connector_Id.TryParse,
                                                                  out Connector_Id ConnectorId,
                                                                  out String ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Type

                if (!ChangeAvailabilityRequestJSON.MapMandatory("type",
                                                                "availability type",
                                                                AvailabilityTypesExtentions.Parse,
                                                                out AvailabilityTypes Type,
                                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ChangeAvailabilityRequest = new ChangeAvailabilityRequest(ConnectorId,
                                                                          Type);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ChangeAvailabilityRequestJSON, e);

                ChangeAvailabilityRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ChangeAvailabilityRequestText, out ChangeAvailabilityRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a change availability request.
        /// </summary>
        /// <param name="ChangeAvailabilityRequestText">The text to be parsed.</param>
        /// <param name="ChangeAvailabilityRequest">The parsed change availability request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                         ChangeAvailabilityRequestText,
                                       out ChangeAvailabilityRequest  ChangeAvailabilityRequest,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                ChangeAvailabilityRequestText = ChangeAvailabilityRequestText?.Trim();

                if (ChangeAvailabilityRequestText.IsNotNullOrEmpty())
                {

                    if (ChangeAvailabilityRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(ChangeAvailabilityRequestText),
                                 out ChangeAvailabilityRequest,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(ChangeAvailabilityRequestText).Root,
                                 out ChangeAvailabilityRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ChangeAvailabilityRequestText, e);
            }

            ChangeAvailabilityRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "changeAvailabilityRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",  ConnectorId.ToString()),
                   new XElement(OCPPNS.OCPPv1_6_CP + "type",         Type.       AsText())

               );

        #endregion

        #region ToJSON(CustomChangeAvailabiliyRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChangeAvailabiliyRequestSerializer">A delegate to serialize custom change availability requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChangeAvailabilityRequest> CustomChangeAvailabiliyRequestSerializer  = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("connectorId",  ConnectorId.ToString()),
                           new JProperty("type",         Type.AsText())
                       );

            return CustomChangeAvailabiliyRequestSerializer != null
                       ? CustomChangeAvailabiliyRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChangeAvailabilityRequest1, ChangeAvailabilityRequest2)

        /// <summary>
        /// Compares two change availability requests for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityRequest1">A change availability request.</param>
        /// <param name="ChangeAvailabilityRequest2">Another change availability request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeAvailabilityRequest ChangeAvailabilityRequest1, ChangeAvailabilityRequest ChangeAvailabilityRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeAvailabilityRequest1, ChangeAvailabilityRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((ChangeAvailabilityRequest1 is null) || (ChangeAvailabilityRequest2 is null))
                return false;

            return ChangeAvailabilityRequest1.Equals(ChangeAvailabilityRequest2);

        }

        #endregion

        #region Operator != (ChangeAvailabilityRequest1, ChangeAvailabilityRequest2)

        /// <summary>
        /// Compares two change availability requests for inequality.
        /// </summary>
        /// <param name="ChangeAvailabilityRequest1">A change availability request.</param>
        /// <param name="ChangeAvailabilityRequest2">Another change availability request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeAvailabilityRequest ChangeAvailabilityRequest1, ChangeAvailabilityRequest ChangeAvailabilityRequest2)

            => !(ChangeAvailabilityRequest1 == ChangeAvailabilityRequest2);

        #endregion

        #endregion

        #region IEquatable<ChangeAvailabilityRequest> Members

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

            if (!(Object is ChangeAvailabilityRequest ChangeAvailabilityRequest))
                return false;

            return Equals(ChangeAvailabilityRequest);

        }

        #endregion

        #region Equals(ChangeAvailabilityRequest)

        /// <summary>
        /// Compares two change availability requests for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityRequest">A change availability request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ChangeAvailabilityRequest ChangeAvailabilityRequest)
        {

            if (ChangeAvailabilityRequest is null)
                return false;

            return ConnectorId.Equals(ChangeAvailabilityRequest.ConnectorId) &&
                   Type.       Equals(ChangeAvailabilityRequest.Type);

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

                return ConnectorId.GetHashCode() * 5 ^
                       Type.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ConnectorId,
                             " / ",
                             Type);

        #endregion

    }

}
