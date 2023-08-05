/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.CSMS
{

    /// <summary>
    /// A notify monitoring report response.
    /// </summary>
    public class NotifyMonitoringReportResponse : AResponse<CS.NotifyMonitoringReportRequest,
                                                            NotifyMonitoringReportResponse>
    {

        #region Constructor(s)

        #region NotifyMonitoringReportResponse(Request, ...)

        /// <summary>
        /// Create a new notify monitoring report response.
        /// </summary>
        /// <param name="Request">The notify monitoring report request leading to this response.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public NotifyMonitoringReportResponse(CS.NotifyMonitoringReportRequest  Request,
                                              CustomData?                       CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        { }

        #endregion

        #region NotifyMonitoringReportResponse(Request, Result)

        /// <summary>
        /// Create a new notify monitoring report response.
        /// </summary>
        /// <param name="Request">The notify monitoring report request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyMonitoringReportResponse(CS.NotifyMonitoringReportRequest  Request,
                                              Result                            Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyMonitoringReportResponse",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomNotifyMonitoringReportResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify monitoring report response.
        /// </summary>
        /// <param name="Request">The notify monitoring report request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyMonitoringReportResponseParser">A delegate to parse custom notify monitoring report responses.</param>
        public static NotifyMonitoringReportResponse Parse(CS.NotifyMonitoringReportRequest                              Request,
                                                           JObject                                                       JSON,
                                                           CustomJObjectParserDelegate<NotifyMonitoringReportResponse>?  CustomNotifyMonitoringReportResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var notifyMonitoringReportResponse,
                         out var errorResponse,
                         CustomNotifyMonitoringReportResponseParser))
            {
                return notifyMonitoringReportResponse!;
            }

            throw new ArgumentException("The given JSON representation of a notify monitoring report response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyMonitoringReportResponse, out ErrorResponse, CustomNotifyMonitoringReportResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify monitoring report response.
        /// </summary>
        /// <param name="Request">The notify monitoring report request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyMonitoringReportResponse">The parsed notify monitoring report response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyMonitoringReportResponseParser">A delegate to parse custom notify monitoring report responses.</param>
        public static Boolean TryParse(CS.NotifyMonitoringReportRequest                              Request,
                                       JObject                                                       JSON,
                                       out NotifyMonitoringReportResponse?                           NotifyMonitoringReportResponse,
                                       out String?                                                   ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyMonitoringReportResponse>?  CustomNotifyMonitoringReportResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                NotifyMonitoringReportResponse = null;

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyMonitoringReportResponse = new NotifyMonitoringReportResponse(Request,
                                                                                    CustomData);

                if (CustomNotifyMonitoringReportResponseParser is not null)
                    NotifyMonitoringReportResponse = CustomNotifyMonitoringReportResponseParser(JSON,
                                                                                                NotifyMonitoringReportResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyMonitoringReportResponse  = null;
                ErrorResponse                   = "The given JSON representation of a notify monitoring report response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyMonitoringReportResponseSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyMonitoringReportResponseSerializer">A delegate to serialize custom notify monitoring report responses.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyMonitoringReportResponse>?  CustomNotifyMonitoringReportResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyMonitoringReportResponseSerializer is not null
                       ? CustomNotifyMonitoringReportResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The notify monitoring report request failed.
        /// </summary>
        public static NotifyMonitoringReportResponse Failed(CS.NotifyMonitoringReportRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (NotifyMonitoringReportResponse1, NotifyMonitoringReportResponse2)

        /// <summary>
        /// Compares two notify monitoring report responses for equality.
        /// </summary>
        /// <param name="NotifyMonitoringReportResponse1">A notify monitoring report response.</param>
        /// <param name="NotifyMonitoringReportResponse2">Another notify monitoring report response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyMonitoringReportResponse? NotifyMonitoringReportResponse1,
                                           NotifyMonitoringReportResponse? NotifyMonitoringReportResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyMonitoringReportResponse1, NotifyMonitoringReportResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyMonitoringReportResponse1 is null || NotifyMonitoringReportResponse2 is null)
                return false;

            return NotifyMonitoringReportResponse1.Equals(NotifyMonitoringReportResponse2);

        }

        #endregion

        #region Operator != (NotifyMonitoringReportResponse1, NotifyMonitoringReportResponse2)

        /// <summary>
        /// Compares two notify monitoring report responses for inequality.
        /// </summary>
        /// <param name="NotifyMonitoringReportResponse1">A notify monitoring report response.</param>
        /// <param name="NotifyMonitoringReportResponse2">Another notify monitoring report response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyMonitoringReportResponse? NotifyMonitoringReportResponse1,
                                           NotifyMonitoringReportResponse? NotifyMonitoringReportResponse2)

            => !(NotifyMonitoringReportResponse1 == NotifyMonitoringReportResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyMonitoringReportResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify monitoring report responses for equality.
        /// </summary>
        /// <param name="Object">A notify monitoring report response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyMonitoringReportResponse notifyMonitoringReportResponse &&
                   Equals(notifyMonitoringReportResponse);

        #endregion

        #region Equals(NotifyMonitoringReportResponse)

        /// <summary>
        /// Compares two notify monitoring report responses for equality.
        /// </summary>
        /// <param name="NotifyMonitoringReportResponse">A notify monitoring report response to compare with.</param>
        public override Boolean Equals(NotifyMonitoringReportResponse? NotifyMonitoringReportResponse)

            => NotifyMonitoringReportResponse is not null &&
                   base.GenericEquals(NotifyMonitoringReportResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "NotifyMonitoringReportResponse";

        #endregion

    }

}
