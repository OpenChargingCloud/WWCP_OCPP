/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A clear monitoring result.
    /// </summary>
    public class ClearMonitoringResult : ACustomData,
                                         IEquatable<ClearMonitoringResult>
    {

        #region Properties

        /// <summary>
        /// The result of the clear request.
        /// </summary>
        [Mandatory]
        public ClearMonitoringStatus  Status        { get; }

        /// <summary>
        /// The unique identification of the variable monitor of which a clear was requested.
        /// </summary>
        [Mandatory]
        public VariableMonitoring_Id  Id            { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?            StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new clear monitoring result.
        /// </summary>
        /// <param name="Status">The result of the clear request.</param>
        /// <param name="Id">The unique identification of the variable monitor of which a clear was requested.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public ClearMonitoringResult(ClearMonitoringStatus  Status,
                                     VariableMonitoring_Id  Id,
                                     StatusInfo?            StatusInfo   = null,
                                     CustomData?            CustomData   = null)

            : base(CustomData)

        {

            this.Status      = Status;
            this.Id          = Id;
            this.StatusInfo  = StatusInfo;

        }

        #endregion


        #region Documentation

        // "ClearMonitoringResultType": {
        //   "javaType": "ClearMonitoringResult",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "status": {
        //       "$ref": "#/definitions/ClearMonitoringStatusEnumType"
        //     },
        //     "id": {
        //       "description": "Id of the monitor of which a clear was requested.\r\n\r\n",
        //       "type": "integer"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     }
        //   },
        //   "required": [
        //     "status",
        //     "id"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomClearMonitoringResultParser = null)

        /// <summary>
        /// Parse the given JSON representation of a clear monitoring result.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearMonitoringResultParser">A delegate to parse custom clear monitoring results.</param>
        public static ClearMonitoringResult Parse(JObject                                             JSON,
                                                  CustomJObjectParserDelegate<ClearMonitoringResult>?  CustomClearMonitoringResultParser   = null)
        {

            if (TryParse(JSON,
                         out var clearMonitoringResult,
                         out var errorResponse,
                         CustomClearMonitoringResultParser) &&
                clearMonitoringResult is not null)
            {
                return clearMonitoringResult;
            }

            throw new ArgumentException("The given JSON representation of a clear monitoring result is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ClearMonitoringResult, CustomClearMonitoringResultParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a clear monitoring result.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearMonitoringResult">The parsed clear monitoring result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                     JSON,
                                       out ClearMonitoringResult?  ClearMonitoringResult,
                                       out String?                 ErrorResponse)

            => TryParse(JSON,
                        out ClearMonitoringResult,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a clear monitoring result.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearMonitoringResult">The parsed clear monitoring result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearMonitoringResultParser">A delegate to parse custom clear monitoring result JSON objects.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       out ClearMonitoringResult?                           ClearMonitoringResult,
                                       out String?                                          ErrorResponse,
                                       CustomJObjectParserDelegate<ClearMonitoringResult>?  CustomClearMonitoringResultParser)
        {

            try
            {

                ClearMonitoringResult = default;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "clear variable monitor status",
                                         ClearMonitoringStatusExtensions.TryParse,
                                         out ClearMonitoringStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Id            [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "monitoring identification",
                                         out UInt64 id,
                                         out ErrorResponse))
                {
                    return false;
                }

                var Id = VariableMonitoring_Id.TryParse(id);

                if (!Id.HasValue)
                    return false;

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
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


                ClearMonitoringResult = new ClearMonitoringResult(Status,
                                                                  Id.Value,
                                                                  StatusInfo,
                                                                  CustomData);

                if (CustomClearMonitoringResultParser is not null)
                    ClearMonitoringResult = CustomClearMonitoringResultParser(JSON,
                                                                              ClearMonitoringResult);

                return true;

            }
            catch (Exception e)
            {
                ClearMonitoringResult  = default;
                ErrorResponse          = "The given JSON representation of a clear monitoring result is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearMonitoringResultSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearMonitoringResultSerializer">A delegate to serialize custom ClearMonitoringResult objects.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearMonitoringResult>?  CustomClearMonitoringResultSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?             CustomStatusInfoSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),
                                 new JProperty("id",           Id.        Value),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearMonitoringResultSerializer is not null
                       ? CustomClearMonitoringResultSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearMonitoringResult1, ClearMonitoringResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearMonitoringResult1">A clear monitoring result.</param>
        /// <param name="ClearMonitoringResult2">Another clear monitoring result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ClearMonitoringResult? ClearMonitoringResult1,
                                           ClearMonitoringResult? ClearMonitoringResult2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearMonitoringResult1, ClearMonitoringResult2))
                return true;

            // If one is null, but not both, return false.
            if (ClearMonitoringResult1 is null || ClearMonitoringResult2 is null)
                return false;

            return ClearMonitoringResult1.Equals(ClearMonitoringResult2);

        }

        #endregion

        #region Operator != (ClearMonitoringResult1, ClearMonitoringResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearMonitoringResult1">A clear monitoring result.</param>
        /// <param name="ClearMonitoringResult2">Another clear monitoring result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ClearMonitoringResult? ClearMonitoringResult1,
                                           ClearMonitoringResult? ClearMonitoringResult2)

            => !(ClearMonitoringResult1 == ClearMonitoringResult2);

        #endregion

        #endregion

        #region IEquatable<ClearMonitoringResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two clear monitoring results for equality.
        /// </summary>
        /// <param name="Object">A clear monitoring result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearMonitoringResult clearMonitoringResult &&
                   Equals(clearMonitoringResult);

        #endregion

        #region Equals(ClearMonitoringResult)

        /// <summary>
        /// Compares two clear monitoring results for equality.
        /// </summary>
        /// <param name="ClearMonitoringResult">A clear monitoring result to compare with.</param>
        public Boolean Equals(ClearMonitoringResult? ClearMonitoringResult)

            => ClearMonitoringResult is not null &&

               Status.Equals(ClearMonitoringResult.Status) &&
               Id.    Equals(ClearMonitoringResult.Id)     &&

             ((StatusInfo is     null && ClearMonitoringResult.StatusInfo is     null) ||
               StatusInfo is not null && ClearMonitoringResult.StatusInfo is not null && StatusInfo.Equals(ClearMonitoringResult.StatusInfo)) &&

               base.  Equals(ClearMonitoringResult);

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

                return Status.     GetHashCode()       * 7 ^
                       Id.         GetHashCode()       * 5 ^
                      (StatusInfo?.GetHashCode() ?? 0) * 3 ^

                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Status,
                             " for ",
                             Id);

        #endregion

    }

}
