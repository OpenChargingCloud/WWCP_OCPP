﻿/*
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
    /// A set charging profile request.
    /// </summary>
    public class SetChargingProfileRequest : ARequest<SetChargingProfileRequest>
    {

        #region Properties

        /// <summary>
        /// The connector to which the charging profile applies.
        /// If connectorId = 0, the message contains an overall
        /// limit for the charge point.
        /// </summary>
        public Connector_Id     ConnectorId        { get; }

        /// <summary>
        /// The charging profile to be set.
        /// </summary>
        public ChargingProfile  ChargingProfile    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a set charging profile request.
        /// </summary>
        /// <param name="ConnectorId">The connector to which the charging profile applies. If connectorId = 0, the message contains an overall limit for the charge point.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
        public SetChargingProfileRequest(Connector_Id     ConnectorId,
                                         ChargingProfile  ChargingProfile)
        {

            this.ConnectorId      = ConnectorId;
            this.ChargingProfile  = ChargingProfile ?? throw new ArgumentNullException(nameof(ChargingProfile),  "The given charging profile must not be null!");

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
        //       <ns:setChargingProfileRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //
        //          <ns:csChargingProfiles>
        //
        //             <ns:chargingProfileId>?</ns:chargingProfileId>
        //
        //             <!--Optional:-->
        //             <ns:transactionId>?</ns:transactionId>
        //
        //             <ns:stackLevel>?</ns:stackLevel>
        //             <ns:chargingProfilePurpose>?</ns:chargingProfilePurpose>
        //             <ns:chargingProfileKind>?</ns:chargingProfileKind>
        //
        //             <!--Optional:-->
        //             <ns:recurrencyKind>?</ns:recurrencyKind>
        //
        //             <!--Optional:-->
        //             <ns:validFrom>?</ns:validFrom>
        //
        //             <!--Optional:-->
        //             <ns:validTo>?</ns:validTo>
        //
        //             <ns:chargingSchedule>
        //
        //                <!--Optional:-->
        //                <ns:duration>?</ns:duration>
        //
        //                <!--Optional:-->
        //                <ns:startSchedule>?</ns:startSchedule>
        //
        //                <ns:chargingRateUnit>?</ns:chargingRateUnit>
        //
        //                <!--1 or more repetitions:-->
        //                <ns:chargingSchedulePeriod>
        //
        //                   <ns:startPeriod>?</ns:startPeriod>
        //                   <ns:limit>?</ns:limit>
        //
        //                   <!--Optional:-->
        //                   <ns:numberPhases>?</ns:numberPhases>
        //
        //                </ns:chargingSchedulePeriod>
        //
        //                <!--Optional:-->
        //                <ns:minChargingRate>?</ns:minChargingRate>
        //
        //             </ns:chargingSchedule>
        //
        //          </ns:csChargingProfiles>
        //
        //       </ns:setChargingProfileRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:SetChargingProfileRequest",
        //     "title":   "SetChargingProfileRequest",
        //     "type":    "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //         "csChargingProfiles": {
        //             "type": "object",
        //             "properties": {
        //                 "chargingProfileId": {
        //                     "type": "integer"
        //                 },
        //                 "transactionId": {
        //                     "type": "integer"
        //                 },
        //                 "stackLevel": {
        //                     "type": "integer"
        //                 },
        //                 "chargingProfilePurpose": {
        //                     "type": "string",
        //                     "additionalProperties": false,
        //                     "enum": [
        //                         "ChargePointMaxProfile",
        //                         "TxDefaultProfile",
        //                         "TxProfile"
        //                     ]
        //                 },
        //                 "chargingProfileKind": {
        //                     "type": "string",
        //                     "additionalProperties": false,
        //                     "enum": [
        //                         "Absolute",
        //                         "Recurring",
        //                         "Relative"
        //                     ]
        //                 },
        //                 "recurrencyKind": {
        //                     "type": "string",
        //                     "additionalProperties": false,
        //                     "enum": [
        //                         "Daily",
        //                         "Weekly"
        //                     ]
        //                 },
        //                 "validFrom": {
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "validTo": {
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "chargingSchedule": {
        //                     "type": "object",
        //                     "properties": {
        //                         "duration": {
        //                             "type": "integer"
        //                         },
        //                         "startSchedule": {
        //                             "type": "string",
        //                             "format": "date-time"
        //                         },
        //                         "chargingRateUnit": {
        //                             "type": "string",
        //                             "additionalProperties": false,
        //                             "enum": [
        //                                 "A",
        //                                 "W"
        //                             ]
        //                         },
        //                         "chargingSchedulePeriod": {
        //                             "type": "array",
        //                             "items": {
        //                                 "type": "object",
        //                                 "properties": {
        //                                     "startPeriod": {
        //                                         "type": "integer"
        //                                     },
        //                                 "limit": {
        //                                     "type": "number",
        //                                     "multipleOf" : 0.1
        //                                 },
        //                                 "numberPhases": {
        //                                         "type": "integer"
        //                                     }
        //                                 },
        //                                 "additionalProperties": false,
        //                                 "required": [
        //                                     "startPeriod",
        //                                     "limit"
        //                                 ]
        //                             }
        //                         },
        //                         "minChargingRate": {
        //                             "type": "number",
        //                             "multipleOf" : 0.1
        //                         }
        //                     },
        //                     "additionalProperties": false,
        //                     "required": [
        //                         "chargingRateUnit",
        //                         "chargingSchedulePeriod"
        //                     ]
        //                 }
        //             },
        //             "additionalProperties": false,
        //             "required": [
        //                 "chargingProfileId",
        //                 "stackLevel",
        //                 "chargingProfilePurpose",
        //                 "chargingProfileKind",
        //                 "chargingSchedule"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "connectorId",
        //         "csChargingProfiles"
        //     ]
        // }

        #endregion

        #region (static) Parse   (SetChargingProfileRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a set charging profile request.
        /// </summary>
        /// <param name="SetChargingProfileRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SetChargingProfileRequest Parse(XElement             SetChargingProfileRequestXML,
                                                      OnExceptionDelegate  OnException = null)
        {

            if (TryParse(SetChargingProfileRequestXML,
                         out SetChargingProfileRequest setChargingProfileRequest,
                         OnException))
            {
                return setChargingProfileRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (SetChargingProfileRequestJSON,  OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a set charging profile request.
        /// </summary>
        /// <param name="SetChargingProfileRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SetChargingProfileRequest Parse(JObject              SetChargingProfileRequestJSON,
                                                      OnExceptionDelegate  OnException = null)
        {

            if (TryParse(SetChargingProfileRequestJSON,
                         out SetChargingProfileRequest setChargingProfileRequest,
                         OnException))
            {
                return setChargingProfileRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (SetChargingProfileRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a set charging profile request.
        /// </summary>
        /// <param name="SetChargingProfileRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SetChargingProfileRequest Parse(String               SetChargingProfileRequestText,
                                                      OnExceptionDelegate  OnException = null)
        {

            if (TryParse(SetChargingProfileRequestText,
                         out SetChargingProfileRequest setChargingProfileRequest,
                         OnException))
            {
                return setChargingProfileRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(SetChargingProfileRequestXML,  out SetChargingProfileRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a set charging profile request.
        /// </summary>
        /// <param name="SetChargingProfileRequestXML">The XML to be parsed.</param>
        /// <param name="SetChargingProfileRequest">The parsed set charging profile request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                       SetChargingProfileRequestXML,
                                       out SetChargingProfileRequest  SetChargingProfileRequest,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                SetChargingProfileRequest = new SetChargingProfileRequest(

                                                SetChargingProfileRequestXML.MapValueOrFail  (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                                              Connector_Id.Parse),

                                                SetChargingProfileRequestXML.MapElementOrFail(OCPPNS.OCPPv1_6_CP + "csChargingProfiles",
                                                                                              ChargingProfile.Parse)

                                            );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, SetChargingProfileRequestXML, e);

                SetChargingProfileRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(SetChargingProfileRequestJSON,  out SetChargingProfileRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a set charging profile request.
        /// </summary>
        /// <param name="SetChargingProfileRequestJSON">The JSON to be parsed.</param>
        /// <param name="SetChargingProfileRequest">The parsed set charging profile request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                        SetChargingProfileRequestJSON,
                                       out SetChargingProfileRequest  SetChargingProfileRequest,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                SetChargingProfileRequest = null;

                #region ConnectorId

                if (!SetChargingProfileRequestJSON.ParseMandatory("connectorId",
                                                                  "connector identification",
                                                                  Connector_Id.TryParse,
                                                                  out Connector_Id  ConnectorId,
                                                                  out String        ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingProfile

                if (!SetChargingProfileRequestJSON.ParseMandatoryJSON2("csChargingProfiles",
                                                                       "charging station charging profiles",
                                                                       OCPPv1_6.ChargingProfile.TryParse,
                                                                       out ChargingProfile ChargingProfile,
                                                                       out                 ErrorResponse))
                {
                    return false;
                }

                #endregion


                SetChargingProfileRequest = new SetChargingProfileRequest(ConnectorId,
                                                                          ChargingProfile);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, SetChargingProfileRequestJSON, e);

                SetChargingProfileRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(SetChargingProfileRequestText, out SetChargingProfileRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a set charging profile request.
        /// </summary>
        /// <param name="SetChargingProfileRequestText">The text to be parsed.</param>
        /// <param name="SetChargingProfileRequest">The parsed set charging profile request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                         SetChargingProfileRequestText,
                                       out SetChargingProfileRequest  SetChargingProfileRequest,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                SetChargingProfileRequestText = SetChargingProfileRequestText?.Trim();

                if (SetChargingProfileRequestText.IsNotNullOrEmpty())
                {

                    if (SetChargingProfileRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(SetChargingProfileRequestText),
                                 out SetChargingProfileRequest,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(SetChargingProfileRequestText).Root,
                                 out SetChargingProfileRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, SetChargingProfileRequestText, e);
            }

            SetChargingProfileRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "setChargingProfileRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",  ConnectorId.ToString()),
                   ChargingProfile.ToXML(OCPPNS.OCPPv1_6_CP + "csChargingProfiles")

               );

        #endregion

        #region ToJSON(CustomSetChargingProfileRequestSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetChargingProfileRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profiles.</param>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedule requests.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetChargingProfileRequest> CustomSetChargingProfileRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingProfile>           CustomChargingProfileSerializer             = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>          CustomChargingScheduleSerializer            = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>    CustomChargingSchedulePeriodSerializer      = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("connectorId",         ConnectorId.    ToString()),
                           new JProperty("csChargingProfiles",  ChargingProfile.ToJSON(CustomChargingProfileSerializer,
                                                                                       CustomChargingScheduleSerializer,
                                                                                       CustomChargingSchedulePeriodSerializer))
                       );

            return CustomSetChargingProfileRequestSerializer != null
                       ? CustomSetChargingProfileRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetChargingProfileRequest1, SetChargingProfileRequest2)

        /// <summary>
        /// Compares two set charging profile requests for equality.
        /// </summary>
        /// <param name="SetChargingProfileRequest1">A set charging profile request.</param>
        /// <param name="SetChargingProfileRequest2">Another set charging profile request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetChargingProfileRequest SetChargingProfileRequest1, SetChargingProfileRequest SetChargingProfileRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetChargingProfileRequest1, SetChargingProfileRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((SetChargingProfileRequest1 is null) || (SetChargingProfileRequest2 is null))
                return false;

            return SetChargingProfileRequest1.Equals(SetChargingProfileRequest2);

        }

        #endregion

        #region Operator != (SetChargingProfileRequest1, SetChargingProfileRequest2)

        /// <summary>
        /// Compares two set charging profile requests for inequality.
        /// </summary>
        /// <param name="SetChargingProfileRequest1">A set charging profile request.</param>
        /// <param name="SetChargingProfileRequest2">Another set charging profile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetChargingProfileRequest SetChargingProfileRequest1, SetChargingProfileRequest SetChargingProfileRequest2)

            => !(SetChargingProfileRequest1 == SetChargingProfileRequest2);

        #endregion

        #endregion

        #region IEquatable<SetChargingProfileRequest> Members

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

            if (!(Object is SetChargingProfileRequest SetChargingProfileRequest))
                return false;

            return Equals(SetChargingProfileRequest);

        }

        #endregion

        #region Equals(SetChargingProfileRequest)

        /// <summary>
        /// Compares two set charging profile requests for equality.
        /// </summary>
        /// <param name="SetChargingProfileRequest">A set charging profile request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(SetChargingProfileRequest SetChargingProfileRequest)
        {

            if (SetChargingProfileRequest is null)
                return false;

            return ConnectorId.    Equals(SetChargingProfileRequest.ConnectorId) &&
                   ChargingProfile.Equals(SetChargingProfileRequest.ChargingProfile);

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

                return ConnectorId.    GetHashCode() * 5 ^
                       ChargingProfile.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat("For ", ConnectorId,
                             " set ", ChargingProfile.ChargingProfileId.ToString());

        #endregion

    }

}
