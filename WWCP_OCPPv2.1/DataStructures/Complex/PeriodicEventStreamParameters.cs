﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Periodic event stream parameters.
    /// </summary>
    public class PeriodicEventStreamParameters : ACustomData,
                                                 IEquatable<PeriodicEventStreamParameters>
    {

        #region Properties

        /// <summary>
        /// The number of items to be sent together in stream.
        /// </summary>
        [Mandatory]
        public UInt32     MaxItems    { get; }

        /// <summary>
        /// The optional time after which stream data is sent.
        /// </summary>
        [Mandatory]
        public TimeSpan?  MaxTime     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new periodic event stream parameters.
        /// </summary>
        /// <param name="MaxItems">The number of items to be sent together in stream.</param>
        /// <param name="MaxTime">The optional time after which stream data is sent.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public PeriodicEventStreamParameters(UInt32       MaxItems,
                                             TimeSpan?    MaxTime,
                                             CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.MaxItems  = MaxItems;
            this.MaxTime   = MaxTime;

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "PeriodicEventStreamParams",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "interval": {
        //             "description": "Time in seconds after which stream data is sent.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "values": {
        //             "description": "Number of items to be sent together in stream.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomPeriodicEventStreamParametersParser = null)

        /// <summary>
        /// Parse the given JSON representation of a periodic event stream parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPeriodicEventStreamParametersParser">An optional delegate to parse custom periodic event stream parameterss.</param>
        public static PeriodicEventStreamParameters Parse(JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<PeriodicEventStreamParameters>?  CustomPeriodicEventStreamParametersParser   = null)
        {

            if (TryParse(JSON,
                         out var periodicEventStreamParams,
                         out var errorResponse,
                         CustomPeriodicEventStreamParametersParser))
            {
                return periodicEventStreamParams;
            }

            throw new ArgumentException("The given JSON representation of a periodic event stream parameters is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(PeriodicEventStreamParametersJSON, out PeriodicEventStreamParameters, out ErrorResponse, CustomPeriodicEventStreamParametersParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a periodic event stream parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PeriodicEventStreamParameters">The parsed periodic event stream parameters.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       [NotNullWhen(true)]  out PeriodicEventStreamParameters?  PeriodicEventStreamParameters,
                                       [NotNullWhen(false)] out String?                         ErrorResponse)

            => TryParse(JSON,
                        out PeriodicEventStreamParameters,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a periodic event stream parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PeriodicEventStreamParameters">The parsed periodic event stream parameters.</param>
        /// <param name="CustomPeriodicEventStreamParametersParser">An optional delegate to parse custom periodic event stream parameterss.</param>
        public static Boolean TryParse(JObject                                                      JSON,
                                       [NotNullWhen(true)]  out PeriodicEventStreamParameters?      PeriodicEventStreamParameters,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       CustomJObjectParserDelegate<PeriodicEventStreamParameters>?  CustomPeriodicEventStreamParametersParser   = null)
        {

            try
            {

                PeriodicEventStreamParameters = default;

                #region MaxItems      [mandatory]

                if (!JSON.ParseMandatory("values",
                                         "values (max items)",
                                         out UInt32 MaxItems,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MaxTime       [optional]

                if (JSON.ParseOptional("interval",
                                       "interval (max time)",
                                       out TimeSpan? MaxTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                PeriodicEventStreamParameters = new PeriodicEventStreamParameters(
                                                    MaxItems,
                                                    MaxTime,
                                                    CustomData
                                                );

                if (CustomPeriodicEventStreamParametersParser is not null)
                    PeriodicEventStreamParameters = CustomPeriodicEventStreamParametersParser(JSON,
                                                                                              PeriodicEventStreamParameters);

                return true;

            }
            catch (Exception e)
            {
                PeriodicEventStreamParameters  = default;
                ErrorResponse                  = "The given JSON representation of a periodic event stream parameters is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPeriodicEventStreamParametersSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPeriodicEventStreamParametersSerializer">A delegate to serialize custom periodic event stream parameterss.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?  CustomPeriodicEventStreamParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("values",       MaxItems),

                           MaxTime.HasValue
                               ? new JProperty("maxTime",      MaxTime.Value.TotalSeconds)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomPeriodicEventStreamParametersSerializer is not null
                       ? CustomPeriodicEventStreamParametersSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (PeriodicEventStreamParameters1, PeriodicEventStreamParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamParameters1">Periodic event stream parameters.</param>
        /// <param name="PeriodicEventStreamParameters2">Other periodic event stream parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PeriodicEventStreamParameters? PeriodicEventStreamParameters1,
                                           PeriodicEventStreamParameters? PeriodicEventStreamParameters2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PeriodicEventStreamParameters1, PeriodicEventStreamParameters2))
                return true;

            // If one is null, but not both, return false.
            if (PeriodicEventStreamParameters1 is null || PeriodicEventStreamParameters2 is null)
                return false;

            return PeriodicEventStreamParameters1.Equals(PeriodicEventStreamParameters2);

        }

        #endregion

        #region Operator != (PeriodicEventStreamParameters1, PeriodicEventStreamParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamParameters1">Periodic event stream parameters.</param>
        /// <param name="PeriodicEventStreamParameters2">Other periodic event stream parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PeriodicEventStreamParameters? PeriodicEventStreamParameters1,
                                           PeriodicEventStreamParameters? PeriodicEventStreamParameters2)

            => !(PeriodicEventStreamParameters1 == PeriodicEventStreamParameters2);

        #endregion

        #endregion

        #region IEquatable<PeriodicEventStreamParameters> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two periodic event stream parameterss for equality.
        /// </summary>
        /// <param name="Object">A periodic event stream parameters to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PeriodicEventStreamParameters periodicEventStreamParams &&
                   Equals(periodicEventStreamParams);

        #endregion

        #region Equals(PeriodicEventStreamParameters)

        /// <summary>
        /// Compares two periodic event stream parameterss for equality.
        /// </summary>
        /// <param name="PeriodicEventStreamParameters">A periodic event stream parameters to compare with.</param>
        public Boolean Equals(PeriodicEventStreamParameters? PeriodicEventStreamParameters)

            => PeriodicEventStreamParameters is not null &&

               MaxItems.Equals(PeriodicEventStreamParameters.MaxItems) &&

            ((!MaxTime.HasValue && !PeriodicEventStreamParameters.MaxTime.HasValue) ||
               MaxTime.HasValue &&  PeriodicEventStreamParameters.MaxTime.HasValue && MaxTime.Value.Equals(PeriodicEventStreamParameters.MaxTime.Value)) &&

               base.    Equals(PeriodicEventStreamParameters);

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

                return MaxItems.GetHashCode()       * 5 ^
                      (MaxTime?.GetHashCode() ?? 0) * 3 ^
                       base.    GetHashCode(); ;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   $"{MaxItems} items",

                   MaxTime.HasValue
                       ? $", {MaxTime.Value.TotalSeconds} seconds"
                       : ""

               );

        #endregion


    }

}
