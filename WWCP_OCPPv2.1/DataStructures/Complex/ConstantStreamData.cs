/*
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
    /// Constant stream data.
    /// </summary>
    public class ConstantStreamData : ACustomData,
                                      IEquatable<ConstantStreamData>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the periodic event stream.
        /// </summary>
        [Mandatory]
        public PeriodicEventStream_Id         Id                      { get; }

        /// <summary>
        /// The periodic event stream parameters.
        /// </summary>
        [Mandatory]
        public PeriodicEventStreamParameters  Parameters              { get; }

        /// <summary>
        /// The variable monitoring identification.
        /// </summary>
        [Mandatory]
        public VariableMonitoring_Id          VariableMonitoringId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new constant stream data.
        /// </summary>
        /// <param name="Id">An unique identification of the periodic event stream.</param>
        /// <param name="Parameters">Periodic event stream parameters.</param>
        /// <param name="VariableMonitoringId">An optional variable monitoring identification.</param>
        public ConstantStreamData(PeriodicEventStream_Id         Id,
                                  PeriodicEventStreamParameters  Parameters,
                                  VariableMonitoring_Id          VariableMonitoringId)
        {

            this.Id                    = Id;
            this.Parameters            = Parameters;
            this.VariableMonitoringId  = VariableMonitoringId;

            unchecked
            {

                hashCode = this.Id.                  GetHashCode() * 7 ^
                           this.Parameters.          GetHashCode() * 5 ^
                           this.VariableMonitoringId.GetHashCode() * 3 ^
                           base.                     GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "ConstantStreamData",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "id": {
        //             "description": "Uniquely identifies the stream",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "params": {
        //             "$ref": "#/definitions/PeriodicEventStreamParamsType"
        //         },
        //         "variableMonitoringId": {
        //             "description": "Id of monitor used to report his event. It can be a preconfigured or hardwired monitor.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "id",
        //         "variableMonitoringId",
        //         "params"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomConstantStreamDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of a constant stream data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomConstantStreamDataParser">A delegate to parse custom constant stream datas.</param>
        public static ConstantStreamData Parse(JObject                                           JSON,
                                               CustomJObjectParserDelegate<ConstantStreamData>?  CustomConstantStreamDataParser   = null)
        {

            if (TryParse(JSON,
                         out var constantStreamData,
                         out var errorResponse,
                         CustomConstantStreamDataParser))
            {
                return constantStreamData;
            }

            throw new ArgumentException("The given JSON representation of a constant stream data is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(ConstantStreamDataJSON, out ConstantStreamData, out ErrorResponse, CustomConstantStreamDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a constant stream data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ConstantStreamData">The parsed constant stream data.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out ConstantStreamData?  ConstantStreamData,
                                       [NotNullWhen(false)] out String?              ErrorResponse)

            => TryParse(JSON,
                        out ConstantStreamData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a constant stream data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ConstantStreamData">The parsed constant stream data.</param>
        /// <param name="CustomConstantStreamDataParser">A delegate to parse custom constant stream datas.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       [NotNullWhen(true)]  out ConstantStreamData?      ConstantStreamData,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       CustomJObjectParserDelegate<ConstantStreamData>?  CustomConstantStreamDataParser   = null)
        {

            try
            {

                ConstantStreamData = default;

                #region Id                      [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "periodic event stream identification",
                                         PeriodicEventStream_Id.TryParse,
                                         out PeriodicEventStream_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parameters              [mandatory]

                if (!JSON.ParseMandatoryJSON("params",
                                             "periodic event stream parameters",
                                             PeriodicEventStreamParameters.TryParse,
                                             out PeriodicEventStreamParameters? Parameters,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region VariableMonitoringId    [mandatory]

                if (!JSON.ParseMandatory("variableMonitoringId",
                                         "variable monitoring identification",
                                         VariableMonitoring_Id.TryParse,
                                         out VariableMonitoring_Id VariableMonitoringId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData              [optional]

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


                ConstantStreamData = new ConstantStreamData(
                                         Id,
                                         Parameters,
                                         VariableMonitoringId
                                     );

                if (CustomConstantStreamDataParser is not null)
                    ConstantStreamData = CustomConstantStreamDataParser(JSON,
                                                                        ConstantStreamData);

                return true;

            }
            catch (Exception e)
            {
                ConstantStreamData  = default;
                ErrorResponse       = "The given JSON representation of a constant stream data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomConstantStreamDataSerializer = null, CustomPeriodicEventStreamParametersSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomConstantStreamDataSerializer">A delegate to serialize custom constant stream datas.</param>
        /// <param name="CustomPeriodicEventStreamParametersSerializer">A delegate to serialize custom periodic event stream parameterss.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ConstantStreamData>?             CustomConstantStreamDataSerializer              = null,
                              CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?  CustomPeriodicEventStreamParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                     Id.                        ToString()),
                                 new JProperty("variableMonitoringId",   VariableMonitoringId.Value.ToString()),
                                 new JProperty("params",                 Parameters.                ToJSON(CustomPeriodicEventStreamParametersSerializer,
                                                                                                           CustomCustomDataSerializer)),

                           CustomData is not null
                               ? new JProperty("customData",             CustomData.                ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomConstantStreamDataSerializer is not null
                       ? CustomConstantStreamDataSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ConstantStreamData1, ConstantStreamData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConstantStreamData1">Constant stream data.</param>
        /// <param name="ConstantStreamData2">Other constant stream data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ConstantStreamData? ConstantStreamData1,
                                           ConstantStreamData? ConstantStreamData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ConstantStreamData1, ConstantStreamData2))
                return true;

            // If one is null, but not both, return false.
            if (ConstantStreamData1 is null || ConstantStreamData2 is null)
                return false;

            return ConstantStreamData1.Equals(ConstantStreamData2);

        }

        #endregion

        #region Operator != (ConstantStreamData1, ConstantStreamData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConstantStreamData1">Constant stream data.</param>
        /// <param name="ConstantStreamData2">Other constant stream data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ConstantStreamData? ConstantStreamData1,
                                           ConstantStreamData? ConstantStreamData2)

            => !(ConstantStreamData1 == ConstantStreamData2);

        #endregion

        #region Operator <  (ConstantStreamData1, ConstantStreamData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConstantStreamData1">Constant stream data.</param>
        /// <param name="ConstantStreamData2">Other constant stream data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ConstantStreamData? ConstantStreamData1,
                                          ConstantStreamData? ConstantStreamData2)
        {

            if (ConstantStreamData1 is null)
                throw new ArgumentNullException(nameof(ConstantStreamData1), "The given constant stream data must not be null!");

            return ConstantStreamData1.CompareTo(ConstantStreamData2) < 0;

        }

        #endregion

        #region Operator <= (ConstantStreamData1, ConstantStreamData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConstantStreamData1">Constant stream data.</param>
        /// <param name="ConstantStreamData2">Other constant stream data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ConstantStreamData? ConstantStreamData1,
                                           ConstantStreamData? ConstantStreamData2)

            => !(ConstantStreamData1 > ConstantStreamData2);

        #endregion

        #region Operator >  (ConstantStreamData1, ConstantStreamData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConstantStreamData1">Constant stream data.</param>
        /// <param name="ConstantStreamData2">Other constant stream data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ConstantStreamData? ConstantStreamData1,
                                          ConstantStreamData? ConstantStreamData2)
        {

            if (ConstantStreamData1 is null)
                throw new ArgumentNullException(nameof(ConstantStreamData1), "The given constant stream data must not be null!");

            return ConstantStreamData1.CompareTo(ConstantStreamData2) > 0;

        }

        #endregion

        #region Operator >= (ConstantStreamData1, ConstantStreamData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConstantStreamData1">Constant stream data.</param>
        /// <param name="ConstantStreamData2">Other constant stream data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ConstantStreamData? ConstantStreamData1,
                                           ConstantStreamData? ConstantStreamData2)

            => !(ConstantStreamData1 < ConstantStreamData2);

        #endregion

        #endregion

        #region IComparable<ConstantStreamData> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two constant stream datas.
        /// </summary>
        /// <param name="Object">Constant stream data to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ConstantStreamData constantStreamData
                   ? CompareTo(constantStreamData)
                   : throw new ArgumentException("The given object is not a constant stream data object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ConstantStreamData)

        /// <summary>
        /// Compares two constant stream datas.
        /// </summary>
        /// <param name="ConstantStreamData">Constant stream data to compare with.</param>
        public Int32 CompareTo(ConstantStreamData? ConstantStreamData)
        {

            if (ConstantStreamData is null)
                throw new ArgumentNullException(nameof(ConstantStreamData),
                                                "The given constant stream data must not be null!");

            var c = Id.                  CompareTo(ConstantStreamData.Id);

            if (c == 0)
                c = VariableMonitoringId.CompareTo(ConstantStreamData.VariableMonitoringId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ConstantStreamData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two constant stream datas for equality.
        /// </summary>
        /// <param name="Object">Constant stream data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ConstantStreamData constantStreamData &&
                   Equals(constantStreamData);

        #endregion

        #region Equals(ConstantStreamData)

        /// <summary>
        /// Compares two constant stream datas for equality.
        /// </summary>
        /// <param name="ConstantStreamData">Constant stream data to compare with.</param>
        public Boolean Equals(ConstantStreamData? ConstantStreamData)

            => ConstantStreamData is not null &&

               Id.                  Equals(ConstantStreamData.Id)                   &&
               Parameters.          Equals(ConstantStreamData.Parameters)           &&
               VariableMonitoringId.Equals(ConstantStreamData.VariableMonitoringId) &&

               base.                Equals(ConstantStreamData);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Id} ({VariableMonitoringId}): {Parameters}";

        #endregion


    }

}
