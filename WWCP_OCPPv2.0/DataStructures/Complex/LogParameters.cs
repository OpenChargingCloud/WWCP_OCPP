/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Log parameters.
    /// </summary>
    public class LogParameters : ACustomData,
                                 IEquatable<LogParameters>
    {

        #region Properties

        /// <summary>
        /// The URL of the location at the remote system where the log should be stored.
        /// </summary>
        [Mandatory]
        public URL        RemoteLocation     { get; }

        /// <summary>
        /// The optional timestamp of the oldest logging information to include in the diagnostics.
        /// </summary>
        [Optional]
        public DateTime?  OldestTimestamp    { get; }

        /// <summary>
        /// The optional timestamp of the latest logging information to include in the diagnostics.
        /// </summary>
        [Optional]
        public DateTime?  LatestTimestamp    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new log parameters.
        /// </summary>
        /// <param name="RemoteLocation">The URL of the location at the remote system where the log should be stored.</param>
        /// <param name="OldestTimestamp">An optional timestamp of the oldest logging information to include in the diagnostics.</param>
        /// <param name="LatestTimestamp">An optional timestamp of the latest logging information to include in the diagnostics.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public LogParameters(URL          RemoteLocation,
                             DateTime?    OldestTimestamp   = null,
                             DateTime?    LatestTimestamp   = null,
                             CustomData?  CustomData        = null)

            : base(CustomData)

        {

            this.RemoteLocation   = RemoteLocation;
            this.OldestTimestamp  = OldestTimestamp;
            this.LatestTimestamp  = LatestTimestamp;

        }

        #endregion


        #region Documentation

        // "LogParametersType": {
        //   "description": "Log\r\nurn:x-enexis:ecdm:uid:2:233373\r\nGeneric class for the configuration of logging entries.\r\n",
        //   "javaType": "LogParameters",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "remoteLocation": {
        //       "description": "Log. Remote_ Location. URI\r\nurn:x-enexis:ecdm:uid:1:569484\r\nThe URL of the location at the remote system where the log should be stored.\r\n",
        //       "type": "string",
        //       "maxLength": 512
        //     },
        //     "oldestTimestamp": {
        //       "description": "Log. Oldest_ Timestamp. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569477\r\nThis contains the date and time of the oldest logging information to include in the diagnostics.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "latestTimestamp": {
        //       "description": "Log. Latest_ Timestamp. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569482\r\nThis contains the date and time of the latest logging information to include in the diagnostics.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     }
        //   },
        //   "required": [
        //     "remoteLocation"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomLogParametersParser = null)

        /// <summary>
        /// Parse the given JSON representation of log parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomLogParametersParser">A delegate to parse custom LogParameterss.</param>
        public static LogParameters Parse(JObject                                      JSON,
                                          CustomJObjectParserDelegate<LogParameters>?  CustomLogParametersParser   = null)
        {

            if (TryParse(JSON,
                         out var logParameters,
                         out var errorResponse,
                         CustomLogParametersParser))
            {
                return logParameters!;
            }

            throw new ArgumentException("The given JSON representation of log parameters is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out LogParameters, out ErrorResponse, CustomLogParametersParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of log parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="LogParameters">The parsed log parameters.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       out LogParameters?  LogParameters,
                                       out String?         ErrorResponse)

            => TryParse(JSON,
                        out LogParameters,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of log parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="LogParameters">The parsed log parameters.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomLogParametersParser">A delegate to parse custom LogParameterss.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       out LogParameters?                           LogParameters,
                                       out String?                                  ErrorResponse,
                                       CustomJObjectParserDelegate<LogParameters>?  CustomLogParametersParser)
        {

            try
            {

                LogParameters = null;

                #region RemoteLocation     [mandatory]

                if (!JSON.ParseMandatory("remoteLocation",
                                         "remote location",
                                         URL.TryParse,
                                         out URL RemoteLocation,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region OldestTimestamp    [optional]

                if (JSON.ParseOptional("oldestTimestamp",
                                       "oldest timestamp",
                                       out DateTime? OldestTimestamp,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region LatestTimestamp    [optional]

                if (JSON.ParseOptional("latestTimestamp",
                                       "latest timestamp",
                                       out DateTime? LatestTimestamp,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData         [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                LogParameters = new LogParameters(RemoteLocation,
                                                  OldestTimestamp,
                                                  LatestTimestamp,
                                                  CustomData);

                if (CustomLogParametersParser is not null)
                    LogParameters = CustomLogParametersParser(JSON,
                                                              LogParameters);

                return true;

            }
            catch (Exception e)
            {
                LogParameters  = default;
                ErrorResponse  = "The given JSON representation of log parameters is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomLogParametersSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLogParametersSerializer">A delegate to serialize custom log parameters.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LogParameters>?  CustomLogParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?     CustomCustomDataSerializer      = null)
        {

            var json = JSONObject.Create(

                           new JProperty("remoteLocation",         RemoteLocation.       ToString()),

                           OldestTimestamp.HasValue
                               ? new JProperty("remoteLocation",   OldestTimestamp.Value.ToIso8601())
                               : null,

                           LatestTimestamp.HasValue
                               ? new JProperty("latestTimestamp",  LatestTimestamp.Value.ToIso8601())
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.           ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomLogParametersSerializer is not null
                       ? CustomLogParametersSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (LogParameters1, LogParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogParameters1">Log parameters.</param>
        /// <param name="LogParameters2">Other log parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (LogParameters? LogParameters1,
                                           LogParameters? LogParameters2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(LogParameters1, LogParameters2))
                return true;

            // If one is null, but not both, return false.
            if (LogParameters1 is null || LogParameters2 is null)
                return false;

            return LogParameters1.Equals(LogParameters2);

        }

        #endregion

        #region Operator != (LogParameters1, LogParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogParameters1">Log parameters.</param>
        /// <param name="LogParameters2">Other log parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (LogParameters? LogParameters1,
                                           LogParameters? LogParameters2)

            => !(LogParameters1 == LogParameters2);

        #endregion

        #endregion

        #region IEquatable<LogParameters> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two log parameters for equality.
        /// </summary>
        /// <param name="Object">A log parameters to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LogParameters logParameters &&
                   Equals(logParameters);

        #endregion

        #region Equals(LogParameters)

        /// <summary>
        /// Compares two log parameters for equality.
        /// </summary>
        /// <param name="LogParameters">A log parameters to compare with.</param>
        public Boolean Equals(LogParameters? LogParameters)

            => LogParameters is not null &&

               RemoteLocation.Equals(LogParameters.RemoteLocation) &&

            ((!OldestTimestamp.HasValue && !LogParameters.OldestTimestamp.HasValue) ||
              (OldestTimestamp.HasValue &&  LogParameters.OldestTimestamp.HasValue && OldestTimestamp.Value.Equals(LogParameters.OldestTimestamp.Value))) &&

            ((!LatestTimestamp.HasValue && !LogParameters.LatestTimestamp.HasValue) ||
              (LatestTimestamp.HasValue &&  LogParameters.LatestTimestamp.HasValue && LatestTimestamp.Value.Equals(LogParameters.LatestTimestamp.Value))) &&

               base.          Equals(LogParameters);

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

                return RemoteLocation.  GetHashCode()       * 7 ^
                      (OldestTimestamp?.GetHashCode() ?? 0) * 5 ^
                      (LatestTimestamp?.GetHashCode() ?? 0) * 3 ^

                      base.             GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(RemoteLocation,
                             OldestTimestamp.HasValue ? ", > " + OldestTimestamp.Value.ToIso8601() : "",
                             LatestTimestamp.HasValue ? ", < " + LatestTimestamp.Value.ToIso8601() : "");

        #endregion

    }

}
