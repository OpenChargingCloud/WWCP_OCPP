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

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// A remote start transaction request.
    /// </summary>
    public class RemoteStartTransactionRequest : ARequest<RemoteStartTransactionRequest>
    {

        #region Properties

        /// <summary>
        /// The identification tag to start the charging transaction.
        /// </summary>
        public IdToken          IdTag              { get; }

        /// <summary>
        /// An optional connector identification on which the charging
        /// transaction should be started (SHALL be > 0).
        /// </summary>
        public Connector_Id?    ConnectorId        { get; }

        /// <summary>
        /// An optional charging profile to be used by the charge point
        /// for the requested charging transaction.
        /// The 'ChargingProfilePurpose' MUST be set to 'TxProfile'.
        /// </summary>
        public ChargingProfile  ChargingProfile    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a RemoteStartTransaction XML/SOAP request.
        /// </summary>
        /// <param name="IdTag">The identification tag to start the charging transaction.</param>
        /// <param name="ConnectorId">An optional connector identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charge point for the requested charging transaction.</param>
        public RemoteStartTransactionRequest(IdToken          IdTag,
                                             Connector_Id?    ConnectorId       = null,
                                             ChargingProfile  ChargingProfile   = null)
        {

            #region Initial checks

            if (IdTag.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(IdTag),  "The given identification tag must not be null!");

            #endregion

            this.IdTag            = IdTag;
            this.ConnectorId      = ConnectorId;
            this.ChargingProfile  = ChargingProfile;

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
        //       <ns:remoteStartTransactionRequest>
        //
        //          <!--Optional:-->
        //          <ns:connectorId>?</ns:connectorId>
        //
        //          <ns:idTag>?</ns:idTag>
        //
        //          <!--Optional:-->
        //          <ns:chargingProfile>
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
        //          </ns:chargingProfile>
        //
        //       </ns:remoteStartTransactionRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:RemoteStartTransactionRequest",
        //     "title":   "RemoteStartTransactionRequest",
        //     "type":    "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //         "idTag": {
        //             "type": "string",
        //             "maxLength": 20
        //         },
        //         "chargingProfile": {
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
        //                                     "limit": {
        //                                         "type": "number",
        //                                         "multipleOf" : 0.1
        //                                     },
        //                                     "numberPhases": {
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
        //         "idTag"
        //     ]
        // }

        #endregion

        #region (static) Parse   (RemoteStartTransactionRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a remote start transaction request.
        /// </summary>
        /// <param name="RemoteStartTransactionRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStartTransactionRequest Parse(XElement             RemoteStartTransactionRequestXML,
                                                          OnExceptionDelegate  OnException = null)
        {

            if (TryParse(RemoteStartTransactionRequestXML,
                         out RemoteStartTransactionRequest remoteStartTransactionRequest,
                         OnException))
            {
                return remoteStartTransactionRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (RemoteStartTransactionRequestJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a remote start transaction request.
        /// </summary>
        /// <param name="RemoteStartTransactionRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStartTransactionRequest Parse(JObject              RemoteStartTransactionRequestJSON,
                                                          OnExceptionDelegate  OnException = null)
        {

            if (TryParse(RemoteStartTransactionRequestJSON,
                         out RemoteStartTransactionRequest remoteStartTransactionRequest,
                         OnException))
            {
                return remoteStartTransactionRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (RemoteStartTransactionRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a remote start transaction request.
        /// </summary>
        /// <param name="RemoteStartTransactionRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStartTransactionRequest Parse(String               RemoteStartTransactionRequestText,
                                                          OnExceptionDelegate  OnException = null)
        {

            if (TryParse(RemoteStartTransactionRequestText,
                         out RemoteStartTransactionRequest remoteStartTransactionRequest,
                         OnException))
            {
                return remoteStartTransactionRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(RemoteStartTransactionRequestXML,  out RemoteStartTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a remote start transaction request.
        /// </summary>
        /// <param name="RemoteStartTransactionRequestXML">The XML to be parsed.</param>
        /// <param name="RemoteStartTransactionRequest">The parsed remote start transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                           RemoteStartTransactionRequestXML,
                                       out RemoteStartTransactionRequest  RemoteStartTransactionRequest,
                                       OnExceptionDelegate                OnException  = null)
        {

            try
            {

                RemoteStartTransactionRequest = new RemoteStartTransactionRequest(

                                                    RemoteStartTransactionRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "idTag",
                                                                                                        IdToken.Parse),

                                                    RemoteStartTransactionRequestXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                                                        Connector_Id.Parse),

                                                    RemoteStartTransactionRequestXML.MapElement        (OCPPNS.OCPPv1_6_CP + "chargingProfile",
                                                                                                        ChargingProfile.Parse)

                                                );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, RemoteStartTransactionRequestXML, e);

                RemoteStartTransactionRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(RemoteStartTransactionRequestJSON, out RemoteStartTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a remote start transaction request.
        /// </summary>
        /// <param name="RemoteStartTransactionRequestJSON">The JSON to be parsed.</param>
        /// <param name="RemoteStartTransactionRequest">The parsed remote start transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                            RemoteStartTransactionRequestJSON,
                                       out RemoteStartTransactionRequest  RemoteStartTransactionRequest,
                                       OnExceptionDelegate                OnException  = null)
        {

            try
            {

                RemoteStartTransactionRequest = null;

                #region IdTag

                if (!RemoteStartTransactionRequestJSON.ParseMandatory("idTag",
                                                                      "identification tag",
                                                                      IdToken.TryParse,
                                                                      out IdToken  IdTag,
                                                                      out String   ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId

                if (RemoteStartTransactionRequestJSON.ParseOptionalStruct("connectorId",
                                                                          "connector identification",
                                                                          Connector_Id.TryParse,
                                                                          out Connector_Id?  ConnectorId,
                                                                          out                ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region ChargingProfile

                if (RemoteStartTransactionRequestJSON.ParseOptionalJSON("chargingProfile",
                                                                        "charging profile",
                                                                        OCPPv1_6.ChargingProfile.TryParse,
                                                                        out ChargingProfile  ChargingProfile,
                                                                        out                  ErrorResponse,
                                                                        OnException))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                RemoteStartTransactionRequest = new RemoteStartTransactionRequest(IdTag,
                                                                                  ConnectorId,
                                                                                  ChargingProfile);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, RemoteStartTransactionRequestJSON, e);

                RemoteStartTransactionRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(RemoteStartTransactionRequestText, out RemoteStartTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a remote start transaction request.
        /// </summary>
        /// <param name="RemoteStartTransactionRequestText">The text to be parsed.</param>
        /// <param name="RemoteStartTransactionRequest">The parsed remote start transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                             RemoteStartTransactionRequestText,
                                       out RemoteStartTransactionRequest  RemoteStartTransactionRequest,
                                       OnExceptionDelegate                OnException  = null)
        {

            try
            {

                RemoteStartTransactionRequestText = RemoteStartTransactionRequestText?.Trim();

                if (RemoteStartTransactionRequestText.IsNotNullOrEmpty())
                {

                    if (RemoteStartTransactionRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(RemoteStartTransactionRequestText),
                                 out RemoteStartTransactionRequest,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(RemoteStartTransactionRequestText).Root,
                                 out RemoteStartTransactionRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, RemoteStartTransactionRequestText, e);
            }

            RemoteStartTransactionRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "remoteStartTransactionRequest",

                   ConnectorId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",  ConnectorId.Value.ToString())
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CP + "idTag",              IdTag.ToString()),

                   ChargingProfile?.ToXML()

               );

        #endregion

        #region ToJSON(CustomRemoteStartTransactionRequestSerializer = null, CustomChargingProfileSerializer = null, CustomChargingScheduleSerializer = null, CustomChargingSchedulePeriodSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoteStartTransactionRequestSerializer">A delegate to serialize custom remote start transaction requests.</param>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profiles.</param>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedule requests.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoteStartTransactionRequest> CustomRemoteStartTransactionRequestSerializer  = null,
                              CustomJObjectSerializerDelegate<ChargingProfile>               CustomChargingProfileSerializer                = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>              CustomChargingScheduleSerializer               = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>        CustomChargingSchedulePeriodSerializer         = null)
        {

            var JSON = JSONObject.Create(

                           ConnectorId.HasValue
                                  ? new JProperty("connectorId",   ConnectorId.Value.ToString())
                                  : null,

                           new JProperty("idTag",                  IdTag.            ToString()),

                           ChargingProfile != null
                               ? new JProperty("chargingProfile",  ChargingProfile.  ToJSON(CustomChargingProfileSerializer,
                                                                                            CustomChargingScheduleSerializer,
                                                                                            CustomChargingSchedulePeriodSerializer))
                               : null

                       );

            return CustomRemoteStartTransactionRequestSerializer != null
                       ? CustomRemoteStartTransactionRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (RemoteStartTransactionRequest1, RemoteStartTransactionRequest2)

        /// <summary>
        /// Compares two remote start transaction requests for equality.
        /// </summary>
        /// <param name="RemoteStartTransactionRequest1">A remote start transaction request.</param>
        /// <param name="RemoteStartTransactionRequest2">Another remote start transaction request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RemoteStartTransactionRequest RemoteStartTransactionRequest1, RemoteStartTransactionRequest RemoteStartTransactionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RemoteStartTransactionRequest1, RemoteStartTransactionRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((RemoteStartTransactionRequest1 is null) || (RemoteStartTransactionRequest2 is null))
                return false;

            return RemoteStartTransactionRequest1.Equals(RemoteStartTransactionRequest2);

        }

        #endregion

        #region Operator != (RemoteStartTransactionRequest1, RemoteStartTransactionRequest2)

        /// <summary>
        /// Compares two remote start transaction requests for inequality.
        /// </summary>
        /// <param name="RemoteStartTransactionRequest1">A remote start transaction request.</param>
        /// <param name="RemoteStartTransactionRequest2">Another remote start transaction request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoteStartTransactionRequest RemoteStartTransactionRequest1, RemoteStartTransactionRequest RemoteStartTransactionRequest2)

            => !(RemoteStartTransactionRequest1 == RemoteStartTransactionRequest2);

        #endregion

        #endregion

        #region IEquatable<RemoteStartTransactionRequest> Members

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

            if (!(Object is RemoteStartTransactionRequest RemoteStartTransactionRequest))
                return false;

            return Equals(RemoteStartTransactionRequest);

        }

        #endregion

        #region Equals(RemoteStartTransactionRequest)

        /// <summary>
        /// Compares two remote start transaction requests for equality.
        /// </summary>
        /// <param name="RemoteStartTransactionRequest">A remote start transaction request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(RemoteStartTransactionRequest RemoteStartTransactionRequest)
        {

            if (RemoteStartTransactionRequest is null)
                return false;

            return IdTag.Equals(RemoteStartTransactionRequest.IdTag) &&

                   ((!ConnectorId.HasValue && !RemoteStartTransactionRequest.ConnectorId.HasValue) ||
                     (ConnectorId.HasValue &&  RemoteStartTransactionRequest.ConnectorId.HasValue && ConnectorId.Value.Equals(RemoteStartTransactionRequest.ConnectorId.Value))) &&

                   ((ChargingProfile == null && RemoteStartTransactionRequest.ChargingProfile == null) ||
                    (ChargingProfile != null && RemoteStartTransactionRequest.ChargingProfile != null && ChargingProfile.Equals(RemoteStartTransactionRequest.ChargingProfile)));

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

                return IdTag.GetHashCode() * 11 ^

                       (ConnectorId.HasValue
                            ? ConnectorId.    GetHashCode() * 7
                            : 0) ^

                       (ChargingProfile != null
                            ? ChargingProfile.GetHashCode() * 5
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(IdTag,

                             ConnectorId.HasValue
                                 ? " at " + IdTag : "",

                             ChargingProfile != null
                                 ? " with profile"
                                 : "");

        #endregion

    }

}
