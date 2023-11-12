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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A periodic event stream parameters.
    /// </summary>
    public readonly struct PeriodicEventStreamParams : IEquatable<PeriodicEventStreamParams>,
                                                       IComparable<PeriodicEventStreamParams>,
                                                       IComparable
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
        public PeriodicEventStreamParams(UInt32     MaxItems,
                                         TimeSpan?  MaxTime)
        {

            this.MaxItems  = MaxItems;
            this.MaxTime   = MaxTime;

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, CustomPeriodicEventStreamParamsParser = null)

        /// <summary>
        /// Parse the given JSON representation of a periodic event stream parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPeriodicEventStreamParamsParser">A delegate to parse custom periodic event stream parameterss.</param>
        public static PeriodicEventStreamParams Parse(JObject                                                  JSON,
                                                      CustomJObjectParserDelegate<PeriodicEventStreamParams>?  CustomPeriodicEventStreamParamsParser   = null)
        {

            if (TryParse(JSON,
                         out var periodicEventStreamParams,
                         out var errorResponse,
                         CustomPeriodicEventStreamParamsParser))
            {
                return periodicEventStreamParams;
            }

            throw new ArgumentException("The given JSON representation of a periodic event stream parameters is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(PeriodicEventStreamParamsJSON, out PeriodicEventStreamParams, out ErrorResponse, CustomPeriodicEventStreamParamsParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a periodic event stream parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PeriodicEventStreamParams">The parsed periodic event stream parameters.</param>
        public static Boolean TryParse(JObject                        JSON,
                                       out PeriodicEventStreamParams  PeriodicEventStreamParams,
                                       out String?                    ErrorResponse)

            => TryParse(JSON,
                        out PeriodicEventStreamParams,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a periodic event stream parameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PeriodicEventStreamParams">The parsed periodic event stream parameters.</param>
        /// <param name="CustomPeriodicEventStreamParamsParser">A delegate to parse custom periodic event stream parameterss.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       out PeriodicEventStreamParams                            PeriodicEventStreamParams,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<PeriodicEventStreamParams>?  CustomPeriodicEventStreamParamsParser   = null)
        {

            try
            {

                PeriodicEventStreamParams = default;

                #region MaxItems    [mandatory]

                if (!JSON.ParseMandatory("maxItems",
                                         "max items",
                                         out UInt32 MaxItems,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MaxTime     [optional]

                if (JSON.ParseOptional("maxTime",
                                       "max time",
                                       out TimeSpan? MaxTime,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion


                PeriodicEventStreamParams = new PeriodicEventStreamParams(
                                                MaxItems,
                                                MaxTime
                                            );

                if (CustomPeriodicEventStreamParamsParser is not null)
                    PeriodicEventStreamParams = CustomPeriodicEventStreamParamsParser(JSON,
                                                                                      PeriodicEventStreamParams);

                return true;

            }
            catch (Exception e)
            {
                PeriodicEventStreamParams  = default;
                ErrorResponse              = "The given JSON representation of a periodic event stream parameters is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPeriodicEventStreamParamsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPeriodicEventStreamParamsSerializer">A delegate to serialize custom periodic event stream parameterss.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PeriodicEventStreamParams>? CustomPeriodicEventStreamParamsSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("maxItems",   MaxItems),

                           MaxTime.HasValue
                               ? new JProperty("maxTime",    MaxTime.Value.TotalSeconds)
                               : null

                       );

            return CustomPeriodicEventStreamParamsSerializer is not null
                       ? CustomPeriodicEventStreamParamsSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (PeriodicEventStreamParams1, PeriodicEventStreamParams2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamParams1">A periodic event stream parameters.</param>
        /// <param name="PeriodicEventStreamParams2">Another periodic event stream parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PeriodicEventStreamParams PeriodicEventStreamParams1,
                                           PeriodicEventStreamParams PeriodicEventStreamParams2)

            => PeriodicEventStreamParams1.Equals(PeriodicEventStreamParams2);

        #endregion

        #region Operator != (PeriodicEventStreamParams1, PeriodicEventStreamParams2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamParams1">A periodic event stream parameters.</param>
        /// <param name="PeriodicEventStreamParams2">Another periodic event stream parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PeriodicEventStreamParams PeriodicEventStreamParams1,
                                           PeriodicEventStreamParams PeriodicEventStreamParams2)

            => !PeriodicEventStreamParams1.Equals(PeriodicEventStreamParams2);

        #endregion

        #region Operator <  (PeriodicEventStreamParams1, PeriodicEventStreamParams2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamParams1">A periodic event stream parameters.</param>
        /// <param name="PeriodicEventStreamParams2">Another periodic event stream parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PeriodicEventStreamParams PeriodicEventStreamParams1,
                                          PeriodicEventStreamParams PeriodicEventStreamParams2)

            => PeriodicEventStreamParams1.CompareTo(PeriodicEventStreamParams2) < 0;

        #endregion

        #region Operator <= (PeriodicEventStreamParams1, PeriodicEventStreamParams2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamParams1">A periodic event stream parameters.</param>
        /// <param name="PeriodicEventStreamParams2">Another periodic event stream parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PeriodicEventStreamParams PeriodicEventStreamParams1,
                                           PeriodicEventStreamParams PeriodicEventStreamParams2)

            => PeriodicEventStreamParams1.CompareTo(PeriodicEventStreamParams2) <= 0;

        #endregion

        #region Operator >  (PeriodicEventStreamParams1, PeriodicEventStreamParams2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamParams1">A periodic event stream parameters.</param>
        /// <param name="PeriodicEventStreamParams2">Another periodic event stream parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PeriodicEventStreamParams PeriodicEventStreamParams1,
                                          PeriodicEventStreamParams PeriodicEventStreamParams2)

            => PeriodicEventStreamParams1.CompareTo(PeriodicEventStreamParams2) > 0;

        #endregion

        #region Operator >= (PeriodicEventStreamParams1, PeriodicEventStreamParams2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PeriodicEventStreamParams1">A periodic event stream parameters.</param>
        /// <param name="PeriodicEventStreamParams2">Another periodic event stream parameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PeriodicEventStreamParams PeriodicEventStreamParams1,
                                           PeriodicEventStreamParams PeriodicEventStreamParams2)

            => PeriodicEventStreamParams1.CompareTo(PeriodicEventStreamParams2) >= 0;

        #endregion

        #endregion

        #region IComparable<PeriodicEventStreamParams> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two periodic event stream parameterss.
        /// </summary>
        /// <param name="Object">A periodic event stream parameters to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PeriodicEventStreamParams periodicEventStreamParams
                   ? CompareTo(periodicEventStreamParams)
                   : throw new ArgumentException("The given object is not a periodic event stream parameters!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PeriodicEventStreamParams)

        /// <summary>
        /// Compares two periodic event stream parameterss.
        /// </summary>
        /// <param name="PeriodicEventStreamParams">A periodic event stream parameters to compare with.</param>
        public Int32 CompareTo(PeriodicEventStreamParams PeriodicEventStreamParams)
        {

            var c = MaxItems.     CompareTo(PeriodicEventStreamParams.MaxItems);

            if (c == 0 && MaxTime.HasValue && PeriodicEventStreamParams.MaxTime.HasValue)
                c = MaxTime.Value.CompareTo(PeriodicEventStreamParams.MaxTime.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<PeriodicEventStreamParams> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two periodic event stream parameterss for equality.
        /// </summary>
        /// <param name="Object">A periodic event stream parameters to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PeriodicEventStreamParams periodicEventStreamParams &&
                   Equals(periodicEventStreamParams);

        #endregion

        #region Equals(PeriodicEventStreamParams)

        /// <summary>
        /// Compares two periodic event stream parameterss for equality.
        /// </summary>
        /// <param name="PeriodicEventStreamParams">A periodic event stream parameters to compare with.</param>
        public Boolean Equals(PeriodicEventStreamParams PeriodicEventStreamParams)

            => MaxItems.Equals(PeriodicEventStreamParams.MaxItems) &&

            ((!MaxTime.HasValue && !PeriodicEventStreamParams.MaxTime.HasValue) ||
               MaxTime.HasValue &&  PeriodicEventStreamParams.MaxTime.HasValue && MaxTime.Value.Equals(PeriodicEventStreamParams.MaxTime.Value));

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

                return MaxItems.GetHashCode() * 3 ^
                       MaxTime?.GetHashCode() ?? 0;

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
