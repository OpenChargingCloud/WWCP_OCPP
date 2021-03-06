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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The ClearChargingProfile request.
    /// </summary>
    public class ClearChargingProfileRequest : ARequest<ClearChargingProfileRequest>
    {

        #region Properties

        /// <summary>
        /// The optional identification of the charging profile to clear.
        /// </summary>
        public ChargingProfile_Id?       ChargingProfileId         { get; }

        /// <summary>
        /// The optional connector for which to clear the charging profiles.
        /// Connector identification 0 specifies the charging profile for the
        /// overall charge point. Absence of this parameter means the clearing
        /// applies to all charging profiles that match the other criteria in
        /// the request.
        /// </summary>
        public Connector_Id?             ConnectorId               { get; }

        /// <summary>
        /// The optional purpose of the charging profiles that will be cleared,
        /// if they meet the other criteria in the request.
        /// </summary>
        public ChargingProfilePurposes?  ChargingProfilePurpose    { get; }

        /// <summary>
        /// The optional stack level for which charging profiles will be cleared,
        /// if they meet the other criteria in the request.
        /// </summary>
        public UInt32?                   StackLevel                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearChargingProfile request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChargingProfileId">The optional identification of the charging profile to clear.</param>
        /// <param name="ConnectorId">The optional connector for which to clear the charging profiles. Connector identification 0 specifies the charging profile for the overall charge point. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.</param>
        /// <param name="ChargingProfilePurpose">The optional purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="StackLevel">The optional stack level for which charging profiles will be cleared, if they meet the other criteria in the request.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public ClearChargingProfileRequest(ChargeBox_Id              ChargeBoxId,
                                           ChargingProfile_Id?       ChargingProfileId        = null,
                                           Connector_Id?             ConnectorId              = null,
                                           ChargingProfilePurposes?  ChargingProfilePurpose   = null,
                                           UInt32?                   StackLevel               = null,

                                           Request_Id?               RequestId                = null,
                                           DateTime?                 RequestTimestamp         = null)

            : base(ChargeBoxId,
                   "ClearChargingProfile",
                   RequestId,
                   RequestTimestamp)

        {

            this.ChargingProfileId       = ChargingProfileId;
            this.ConnectorId             = ConnectorId;
            this.ChargingProfilePurpose  = ChargingProfilePurpose;
            this.StackLevel              = StackLevel;

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
        //       <ns:clearChargingProfileRequest>
        //
        //          <!--Optional:-->
        //          <ns:id>?</ns:id>
        //
        //          <!--Optional:-->
        //          <ns:connectorId>?</ns:connectorId>
        //
        //          <!--Optional:-->
        //          <ns:chargingProfilePurpose>?</ns:chargingProfilePurpose>
        //
        //          <!--Optional:-->
        //          <ns:stackLevel>?</ns:stackLevel>
        //
        //       </ns:clearChargingProfileRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ClearChargingProfileRequest",
        //     "title":   "ClearChargingProfileRequest",
        //     "type":    "object",
        //     "properties": {
        //         "id": {
        //             "type": "integer"
        //         },
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //         "chargingProfilePurpose": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "ChargePointMaxProfile",
        //                 "TxDefaultProfile",
        //                 "TxProfile"
        //             ]
        //         },
        //         "stackLevel": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearChargingProfileRequest Parse(XElement             XML,
                                                        Request_Id           RequestId,
                                                        ChargeBox_Id         ChargeBoxId,
                                                        OnExceptionDelegate  OnException = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out ClearChargingProfileRequest clearChargingProfileRequest,
                         OnException))
            {
                return clearChargingProfileRequest;
            }

            throw new ArgumentException("The given XML representation of a ClearChargingProfile request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomClearChargingProfileRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomClearChargingProfileRequestParser">A delegate to parse custom ClearChargingProfile requests.</param>
        public static ClearChargingProfileRequest Parse(JObject                                                   JSON,
                                                        Request_Id                                                RequestId,
                                                        ChargeBox_Id                                              ChargeBoxId,
                                                        CustomJObjectParserDelegate<ClearChargingProfileRequest>  CustomClearChargingProfileRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out ClearChargingProfileRequest  clearChargingProfileRequest,
                         out String                       ErrorResponse,
                         CustomClearChargingProfileRequestParser))
            {
                return clearChargingProfileRequest;
            }

            throw new ArgumentException("The given JSON representation of a ClearChargingProfile request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearChargingProfileRequest Parse(String               Text,
                                                        Request_Id           RequestId,
                                                        ChargeBox_Id         ChargeBoxId,
                                                        OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out ClearChargingProfileRequest clearChargingProfileRequest,
                         OnException))
            {
                return clearChargingProfileRequest;
            }

            throw new ArgumentException("The given text representation of a ClearChargingProfile request is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out ClearChargingProfileRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ClearChargingProfileRequest">The parsed ClearChargingProfile request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                         XML,
                                       Request_Id                       RequestId,
                                       ChargeBox_Id                     ChargeBoxId,
                                       out ClearChargingProfileRequest  ClearChargingProfileRequest,
                                       OnExceptionDelegate              OnException  = null)
        {

            try
            {

                ClearChargingProfileRequest = new ClearChargingProfileRequest(

                                                  ChargeBoxId,

                                                  XML.MapValueOrNull    (OCPPNS.OCPPv1_6_CP + "id",
                                                                         ChargingProfile_Id.Parse),

                                                  XML.MapValueOrNull    (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                         Connector_Id.Parse),

                                                  XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "chargingProfilePurpose",
                                                                         ChargingProfilePurposesExtentions.Parse),

                                                  XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "stackLevel",
                                                                         UInt32.Parse),

                                                  RequestId

                                              );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, XML, e);

                ClearChargingProfileRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out ClearChargingProfileRequest, out ErrorResponse, CustomClearChargingProfileRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ClearChargingProfileRequest">The parsed ClearChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                          JSON,
                                       Request_Id                       RequestId,
                                       ChargeBox_Id                     ChargeBoxId,
                                       out ClearChargingProfileRequest  ClearChargingProfileRequest,
                                       out String                       ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out ClearChargingProfileRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ClearChargingProfileRequest">The parsed ClearChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearChargingProfileRequestParser">A delegate to parse custom ClearChargingProfile requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       ChargeBox_Id                                              ChargeBoxId,
                                       out ClearChargingProfileRequest                           ClearChargingProfileRequest,
                                       out String                                                ErrorResponse,
                                       CustomJObjectParserDelegate<ClearChargingProfileRequest>  CustomClearChargingProfileRequestParser)
        {

            try
            {

                ClearChargingProfileRequest = null;

                #region ChargingProfileId         [optional]

                if (JSON.ParseOptionalStruct("chargingProfileId",
                                             "charging profile identification",
                                             ChargingProfile_Id.TryParse,
                                             out ChargingProfile_Id? ChargingProfileId,
                                             out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region ConnectorId               [optional]

                if (JSON.ParseOptionalStruct("connectorId",
                                             "connector identification",
                                             Connector_Id.TryParse,
                                             out Connector_Id? ConnectorId,
                                             out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region ChargingProfilePurpose    [optional]

                if (JSON.ParseOptional("chargingProfilePurpose",
                                       "charging profile purpose",
                                       ChargingProfilePurposesExtentions.Parse,
                                       out ChargingProfilePurposes? ChargingProfilePurpose,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region StackLevel                [optional]

                if (JSON.ParseOptional("stackLevel",
                                       "stack level",
                                       out UInt32? StackLevel,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region ChargeBoxId               [optional, OCPP_CSE]

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


                ClearChargingProfileRequest = new ClearChargingProfileRequest(ChargeBoxId,
                                                                              ChargingProfileId,
                                                                              ConnectorId,
                                                                              ChargingProfilePurpose,
                                                                              StackLevel,
                                                                              RequestId);

                if (CustomClearChargingProfileRequestParser != null)
                    ClearChargingProfileRequest = CustomClearChargingProfileRequestParser(JSON,
                                                                                          ClearChargingProfileRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearChargingProfileRequest  = default;
                ErrorResponse                = "The given JSON representation of a ClearChargingProfile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(ClearChargingProfileRequestText, RequestId, ChargeBoxId, out ClearChargingProfileRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="ClearChargingProfileRequestText">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ClearChargingProfileRequest">The parsed ClearChargingProfile request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                           ClearChargingProfileRequestText,
                                       Request_Id                       RequestId,
                                       ChargeBox_Id                     ChargeBoxId,
                                       out ClearChargingProfileRequest  ClearChargingProfileRequest,
                                       OnExceptionDelegate              OnException  = null)
        {

            try
            {

                ClearChargingProfileRequestText = ClearChargingProfileRequestText?.Trim();

                if (ClearChargingProfileRequestText.IsNotNullOrEmpty())
                {

                    if (ClearChargingProfileRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(ClearChargingProfileRequestText),
                                 RequestId,
                                 ChargeBoxId,
                                 out ClearChargingProfileRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(ClearChargingProfileRequestText).Root,
                                 RequestId,
                                 ChargeBoxId,
                                 out ClearChargingProfileRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ClearChargingProfileRequestText, e);
            }

            ClearChargingProfileRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "clearChargingProfileRequest",

                   ChargingProfileId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "id",                      ChargingProfileId.ToString())
                       : null,

                   ConnectorId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",             ConnectorId.      ToString())
                       : null,

                   ChargingProfilePurpose.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "chargingProfilePurpose",  ChargingProfilePurpose.Value.AsText())
                       : null,

                   StackLevel.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "stackLevel",              StackLevel.Value)
                       : null

               );

        #endregion

        #region ToJSON(CustomClearChargingProfileRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearChargingProfileRequestSerializer">A delegate to serialize custom ClearChargingProfile requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearChargingProfileRequest> CustomClearChargingProfileRequestSerializer = null)
        {

            var JSON = JSONObject.Create(

                           ChargingProfileId.HasValue
                               ? new JProperty("chargingProfileId",       ChargingProfileId.     Value.ToString())
                               : null,

                           ConnectorId.HasValue
                               ? new JProperty("connectorId",             ConnectorId.           Value.ToString())
                               : null,

                           ChargingProfilePurpose.HasValue
                               ? new JProperty("chargingProfilePurpose",  ChargingProfilePurpose.Value.ToString())
                               : null,

                           StackLevel.HasValue
                               ? new JProperty("stackLevel",              StackLevel.            Value.ToString())
                               : null

                       );

            return CustomClearChargingProfileRequestSerializer != null
                       ? CustomClearChargingProfileRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two ClearChargingProfile requests for equality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A ClearChargingProfile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another ClearChargingProfile request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearChargingProfileRequest ClearChargingProfileRequest1, ClearChargingProfileRequest ClearChargingProfileRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearChargingProfileRequest1, ClearChargingProfileRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((ClearChargingProfileRequest1 is null) || (ClearChargingProfileRequest2 is null))
                return false;

            return ClearChargingProfileRequest1.Equals(ClearChargingProfileRequest2);

        }

        #endregion

        #region Operator != (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two ClearChargingProfile requests for inequality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A ClearChargingProfile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another ClearChargingProfile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearChargingProfileRequest ClearChargingProfileRequest1, ClearChargingProfileRequest ClearChargingProfileRequest2)

            => !(ClearChargingProfileRequest1 == ClearChargingProfileRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfileRequest> Members

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

            if (!(Object is ClearChargingProfileRequest ClearChargingProfileRequest))
                return false;

            return Equals(ClearChargingProfileRequest);

        }

        #endregion

        #region Equals(ClearChargingProfileRequest)

        /// <summary>
        /// Compares two ClearChargingProfile requests for equality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest">A ClearChargingProfile request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ClearChargingProfileRequest ClearChargingProfileRequest)
        {

            if (ClearChargingProfileRequest is null)
                return false;

            return ((ChargingProfileId == null && ClearChargingProfileRequest.ChargingProfileId == null) ||
                    (ChargingProfileId != null && ClearChargingProfileRequest.ChargingProfileId != null && ChargingProfileId.Equals(ClearChargingProfileRequest.ChargingProfileId))) &&

                   ((ConnectorId       == null && ClearChargingProfileRequest.ConnectorId       == null) ||
                    (ConnectorId       != null && ClearChargingProfileRequest.ConnectorId       != null && ConnectorId.      Equals(ClearChargingProfileRequest.ConnectorId))) &&

                   ((!ChargingProfilePurpose.HasValue && !ClearChargingProfileRequest.ChargingProfilePurpose.HasValue) ||
                     (ChargingProfilePurpose.HasValue &&  ClearChargingProfileRequest.ChargingProfilePurpose.HasValue && ChargingProfilePurpose.Value.Equals(ClearChargingProfileRequest.ChargingProfilePurpose.Value))) &&

                   ((!StackLevel.            HasValue && !ClearChargingProfileRequest.StackLevel.            HasValue) ||
                     (StackLevel.            HasValue &&  ClearChargingProfileRequest.StackLevel.            HasValue && StackLevel.Value.Equals(ClearChargingProfileRequest.StackLevel.Value)));

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

                return (ChargingProfileId != null
                            ? ChargingProfileId.     GetHashCode() * 11
                            : 0) ^

                       (ConnectorId != null
                            ? ConnectorId.           GetHashCode() * 7
                            : 0) ^

                       (ChargingProfilePurpose.HasValue
                            ? ChargingProfilePurpose.GetHashCode() * 5
                            : 0) ^

                       (StackLevel.HasValue
                            ? StackLevel.            GetHashCode() * 3
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ChargingProfileId != null
                                 ? ChargingProfileId.ToString()
                                 : "",

                             ConnectorId != null
                                 ? " at " + ConnectorId
                                 : "",

                             ChargingProfilePurpose.HasValue
                                 ? " having " + ChargingProfilePurpose.Value
                                 : "",

                             StackLevel.HasValue
                                 ? " at " + StackLevel.Value
                                 : "");

        #endregion

    }

}
