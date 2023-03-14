/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP <https://github.com/OpenChargingCloud/WWCP_OCPP>
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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A log status notification response.
    /// </summary>
    public class LogStatusNotificationResponse : AResponse<CP.LogStatusNotificationRequest,
                                                              LogStatusNotificationResponse>
    {

        #region Constructor(s)

        #region LogStatusNotificationResponse(Request)

        /// <summary>
        /// Create a new log status notification response.
        /// </summary>
        /// <param name="Request">The log status notification request leading to this response.</param>
        public LogStatusNotificationResponse(CP.LogStatusNotificationRequest Request)

            : base(Request,
                   Result.OK())

        { }

        #endregion

        #region LogStatusNotificationResponse(Request, Result)

        /// <summary>
        /// Create a new log status notification response.
        /// </summary>
        /// <param name="Request">The log status notification request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public LogStatusNotificationResponse(CP.LogStatusNotificationRequest  Request,
                                             Result                           Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:LogStatusNotification.conf",
        //   "type": "object",
        //   "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, LogStatusNotificationResponseJSON, CustomLogStatusNotificationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a log status notification response.
        /// </summary>
        /// <param name="Request">The log status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomLogStatusNotificationResponseParser">A delegate to parse custom log status notification responses.</param>
        public static LogStatusNotificationResponse Parse(CP.LogStatusNotificationRequest                              Request,
                                                          JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<LogStatusNotificationResponse>?  CustomLogStatusNotificationResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var logStatusNotificationResponse,
                         out var errorResponse,
                         CustomLogStatusNotificationResponseParser))
            {
                return logStatusNotificationResponse!;
            }

            throw new ArgumentException("The given JSON representation of a log status notification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out LogStatusNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a log status notification response.
        /// </summary
        /// <param name="Request">The log status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="LogStatusNotificationResponse">The parsed log status notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomLogStatusNotificationResponseParser">A delegate to parse custom log status notification responses.</param>
        public static Boolean TryParse(CP.LogStatusNotificationRequest                              Request,
                                       JObject                                                      JSON,
                                       out LogStatusNotificationResponse?                           LogStatusNotificationResponse,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<LogStatusNotificationResponse>?  CustomLogStatusNotificationResponseParser   = null)
        {

            try
            {

                LogStatusNotificationResponse  = new LogStatusNotificationResponse(Request);
                ErrorResponse                  = null;

                if (CustomLogStatusNotificationResponseParser is not null)
                    LogStatusNotificationResponse = CustomLogStatusNotificationResponseParser(JSON,
                                                                                              LogStatusNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                LogStatusNotificationResponse  = null;
                ErrorResponse                  = "The given JSON representation of a log status notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomLogStatusNotificationResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLogStatusNotificationResponseSerializer">A delegate to serialize custom log status notification responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LogStatusNotificationResponse>? CustomLogStatusNotificationResponseSerializer = null)
        {

            var json = JSONObject.Create();

            return CustomLogStatusNotificationResponseSerializer is not null
                       ? CustomLogStatusNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The log status notification failed.
        /// </summary>
        /// <param name="Request">The log status notification request leading to this response.</param>
        public static LogStatusNotificationResponse Failed(CP.LogStatusNotificationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (LogStatusNotificationResponse1, LogStatusNotificationResponse2)

        /// <summary>
        /// Compares two log status notification responses for equality.
        /// </summary>
        /// <param name="LogStatusNotificationResponse1">A log status notification response.</param>
        /// <param name="LogStatusNotificationResponse2">Another log status notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (LogStatusNotificationResponse? LogStatusNotificationResponse1,
                                           LogStatusNotificationResponse? LogStatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(LogStatusNotificationResponse1, LogStatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (LogStatusNotificationResponse1 is null || LogStatusNotificationResponse2 is null)
                return false;

            return LogStatusNotificationResponse1.Equals(LogStatusNotificationResponse2);

        }

        #endregion

        #region Operator != (LogStatusNotificationResponse1, LogStatusNotificationResponse2)

        /// <summary>
        /// Compares two log status notification responses for inequality.
        /// </summary>
        /// <param name="LogStatusNotificationResponse1">A log status notification response.</param>
        /// <param name="LogStatusNotificationResponse2">Another log status notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (LogStatusNotificationResponse? LogStatusNotificationResponse1,
                                           LogStatusNotificationResponse? LogStatusNotificationResponse2)

            => !(LogStatusNotificationResponse1 == LogStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<LogStatusNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two log status notification responses for equality.
        /// </summary>
        /// <param name="Object">A log status notification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LogStatusNotificationResponse logStatusNotificationResponse &&
                   Equals(logStatusNotificationResponse);

        #endregion

        #region Equals(LogStatusNotificationResponse)

        /// <summary>
        /// Compares two log status notification responses for equality.
        /// </summary>
        /// <param name="LogStatusNotificationResponse">A log status notification response to compare with.</param>
        public override Boolean Equals(LogStatusNotificationResponse? LogStatusNotificationResponse)

            => LogStatusNotificationResponse is not null;

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

            => "log status notification response";

        #endregion

    }

}
