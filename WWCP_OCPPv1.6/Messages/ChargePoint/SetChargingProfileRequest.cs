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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The SetChargingProfile request.
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
        /// Create a new SetChargingProfile request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The connector to which the charging profile applies. If connectorId = 0, the message contains an overall limit for the charge point.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public SetChargingProfileRequest(ChargeBox_Id     ChargeBoxId,
                                         Connector_Id     ConnectorId,
                                         ChargingProfile  ChargingProfile,

                                         Request_Id?      RequestId          = null,
                                         DateTime?        RequestTimestamp   = null,
                                       EventTracking_Id  EventTrackingId           = null)

            : base(ChargeBoxId,
                   "SetChargingProfile",
                   RequestId,
                   EventTrackingId,
                   RequestTimestamp)

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

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of a SetChargingProfile request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SetChargingProfileRequest Parse(XElement             XML,
                                                      Request_Id           RequestId,
                                                      ChargeBox_Id         ChargeBoxId,
                                                      OnExceptionDelegate  OnException = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out SetChargingProfileRequest setChargingProfileRequest,
                         OnException))
            {
                return setChargingProfileRequest;
            }

            throw new ArgumentException("The given XML representation of a SetChargingProfile request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomSetChargingProfileRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomSetChargingProfileRequestParser">A delegate to parse custom SetChargingProfile requests.</param>
        public static SetChargingProfileRequest Parse(JObject                                                 JSON,
                                                      Request_Id                                              RequestId,
                                                      ChargeBox_Id                                            ChargeBoxId,
                                                      CustomJObjectParserDelegate<SetChargingProfileRequest>  CustomSetChargingProfileRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out SetChargingProfileRequest  setChargingProfileRequest,
                         out String                     ErrorResponse,
                         CustomSetChargingProfileRequestParser))
            {
                return setChargingProfileRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetChargingProfile request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of a SetChargingProfile request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SetChargingProfileRequest Parse(String               Text,
                                                      Request_Id           RequestId,
                                                      ChargeBox_Id         ChargeBoxId,
                                                      OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out SetChargingProfileRequest setChargingProfileRequest,
                         OnException))
            {
                return setChargingProfileRequest;
            }

            throw new ArgumentException("The given text representation of a SetChargingProfile request is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out SetChargingProfileRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a SetChargingProfile request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SetChargingProfileRequest">The parsed SetChargingProfile request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                       XML,
                                       Request_Id                     RequestId,
                                       ChargeBox_Id                   ChargeBoxId,
                                       out SetChargingProfileRequest  SetChargingProfileRequest,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                SetChargingProfileRequest = new SetChargingProfileRequest(

                                                ChargeBoxId,

                                                XML.MapValueOrFail  (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                     Connector_Id.Parse),

                                                XML.MapElementOrFail(OCPPNS.OCPPv1_6_CP + "csChargingProfiles",
                                                                     ChargingProfile.Parse),

                                                RequestId

                                            );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, XML, e);

                SetChargingProfileRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out SetChargingProfileRequest, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a SetChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SetChargingProfileRequest">The parsed SetChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                        JSON,
                                       Request_Id                     RequestId,
                                       ChargeBox_Id                   ChargeBoxId,
                                       out SetChargingProfileRequest  SetChargingProfileRequest,
                                       out String                     ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out SetChargingProfileRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a SetChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SetChargingProfileRequest">The parsed SetChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetChargingProfileRequestParser">A delegate to parse custom SetChargingProfile requests.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       ChargeBox_Id                                            ChargeBoxId,
                                       out SetChargingProfileRequest                           SetChargingProfileRequest,
                                       out String                                              ErrorResponse,
                                       CustomJObjectParserDelegate<SetChargingProfileRequest>  CustomSetChargingProfileRequestParser)
        {

            try
            {

                SetChargingProfileRequest = null;

                #region ConnectorId        [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingProfile    [mandatory]

                if (!JSON.ParseMandatoryJSON2("csChargingProfiles",
                                              "charging station charging profiles",
                                              OCPPv1_6.ChargingProfile.TryParse,
                                              out ChargingProfile ChargingProfile,
                                              out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargeBoxId        [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                SetChargingProfileRequest = new SetChargingProfileRequest(ChargeBoxId,
                                                                          ConnectorId,
                                                                          ChargingProfile,
                                                                          RequestId);

                if (CustomSetChargingProfileRequestParser != null)
                    SetChargingProfileRequest = CustomSetChargingProfileRequestParser(JSON,
                                                                                      SetChargingProfileRequest);

                return true;

            }
            catch (Exception e)
            {
                SetChargingProfileRequest  = default;
                ErrorResponse              = "The given JSON representation of a SetChargingProfile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(SetChargingProfileRequestText, RequestId, ChargeBoxId, out SetChargingProfileRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a SetChargingProfile request.
        /// </summary>
        /// <param name="SetChargingProfileRequestText">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="SetChargingProfileRequest">The parsed SetChargingProfile request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                         SetChargingProfileRequestText,
                                       Request_Id                     RequestId,
                                       ChargeBox_Id                   ChargeBoxId,
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
                                 RequestId,
                                 ChargeBoxId,
                                 out SetChargingProfileRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(SetChargingProfileRequestText).Root,
                                 RequestId,
                                 ChargeBoxId,
                                 out SetChargingProfileRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, SetChargingProfileRequestText, e);
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
        /// Compares two SetChargingProfile requests for equality.
        /// </summary>
        /// <param name="SetChargingProfileRequest1">A SetChargingProfile request.</param>
        /// <param name="SetChargingProfileRequest2">Another SetChargingProfile request.</param>
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
        /// Compares two SetChargingProfile requests for inequality.
        /// </summary>
        /// <param name="SetChargingProfileRequest1">A SetChargingProfile request.</param>
        /// <param name="SetChargingProfileRequest2">Another SetChargingProfile request.</param>
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
        /// Compares two SetChargingProfile requests for equality.
        /// </summary>
        /// <param name="SetChargingProfileRequest">A SetChargingProfile request to compare with.</param>
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
