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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The result of a ClearTariffs request.
    /// </summary>
    public class ClearTariffsResult : IEquatable<ClearTariffsResult>,
                                      IComparable<ClearTariffsResult>,
                                      IComparable
    {

        #region Properties

        /// <summary>
        /// The tariff identification.
        /// </summary>
        [Mandatory]
        public Tariff_Id     TariffId      { get; }

        /// <summary>
        /// The ClearTariffs status.
        /// </summary>
        [Mandatory]
        public TariffStatus  Status        { get; }

        /// <summary>
        /// An optional element providing more information about the ClearTariffs status.
        /// </summary>
        [Optional]
        public StatusInfo?   StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new assignments of tariffs to EVSEs or IdTokens.
        /// </summary>
        /// <param name="TariffId">The tariff identification.</param>
        /// <param name="Status">The ClearTariffs status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the ClearTariffs status.</param>
        public ClearTariffsResult(Tariff_Id     TariffId,
                                  TariffStatus  Status,
                                  StatusInfo?   StatusInfo = null)
        {

            this.TariffId    = TariffId;
            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

            unchecked
            {

                hashCode = this.TariffId.   GetHashCode() * 5 ^
                           this.Status.     GetHashCode() * 3 ^
                          (this.StatusInfo?.GetHashCode() ?? 0);

            }

        }

        #endregion


        #region (static) Parse    (JSON, CustomClearTariffsResultParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ClearTariffsResult.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomClearTariffsResultParser">An optional delegate to parse custom ClearTariffsResult JSON objects.</param>
        public static ClearTariffsResult Parse(JObject                                           JSON,
                                               CustomJObjectParserDelegate<ClearTariffsResult>?  CustomClearTariffsResultParser   = null,
                                               CustomJObjectParserDelegate<StatusInfo>?          CustomStatusInfoParser           = null,
                                               CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            if (TryParse(JSON,
                         out var clearTariffsResult,
                         out var errorResponse,
                         CustomClearTariffsResultParser,
                         CustomStatusInfoParser,
                         CustomCustomDataParser))
            {
                return clearTariffsResult;
            }

            throw new ArgumentException("The given JSON representation of a ClearTariffsResult is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse (JSON, out ClearTariffsResult, out ErrorResponse, CustomClearTariffsResultParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a ClearTariffsResult.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ClearTariffsResult">The parsed ClearTariffsResult.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out ClearTariffsResult?  ClearTariffsResult,
                                       [NotNullWhen(false)] out String?              ErrorResponse)

            => TryParse(JSON,
                        out ClearTariffsResult,
                        out ErrorResponse,
                        null,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a ClearTariffsResult.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ClearTariffsResult">The parsed ClearTariffsResult.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearTariffsResultParser">An optional delegate to parse custom ClearTariffsResult JSON objects.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       [NotNullWhen(true)]  out ClearTariffsResult?      ClearTariffsResult,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       CustomJObjectParserDelegate<ClearTariffsResult>?  CustomClearTariffsResultParser,
                                       CustomJObjectParserDelegate<StatusInfo>?          CustomStatusInfoParser,
                                       CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser)
        {

            try
            {

                ClearTariffsResult = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse TariffId      [mandatory]

                if (!JSON.ParseMandatory("tariffId",
                                         "tariff identification",
                                         Tariff_Id.TryParse,
                                         out Tariff_Id TariffId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "ClearTariffs status",
                                         TariffStatus.TryParse,
                                         out TariffStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ClearTariffsResult = new ClearTariffsResult(
                                         TariffId,
                                         Status,
                                         StatusInfo
                                     );


                if (CustomClearTariffsResultParser is not null)
                    ClearTariffsResult = CustomClearTariffsResultParser(JSON,
                                                                        ClearTariffsResult);

                return true;

            }
            catch (Exception e)
            {
                ClearTariffsResult  = default;
                ErrorResponse       = "The given JSON representation of a ClearTariffsResult is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearTariffsResultSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearTariffsResultSerializer">A delegate to serialize custom ClearTariffsResult JSON objects.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearTariffsResult>? CustomClearTariffsResultSerializer = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?         CustomStatusInfoSerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("tariffId",     TariffId.  ToString()),
                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearTariffsResultSerializer is not null
                       ? CustomClearTariffsResultSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public ClearTariffsResult Clone()

            => new (
                   TariffId.Clone,
                   Status,
                   StatusInfo?.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (ClearTariffsResult1, ClearTariffsResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearTariffsResult1">An ClearTariffsResult.</param>
        /// <param name="ClearTariffsResult2">Another ClearTariffsResult.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ClearTariffsResult ClearTariffsResult1,
                                           ClearTariffsResult ClearTariffsResult2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearTariffsResult1, ClearTariffsResult2))
                return true;

            // If one is null, but not both, return false.
            if (ClearTariffsResult1 is null || ClearTariffsResult2 is null)
                return false;

            return ClearTariffsResult1.Equals(ClearTariffsResult2);

        }

        #endregion

        #region Operator != (ClearTariffsResult1, ClearTariffsResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearTariffsResult1">An ClearTariffsResult.</param>
        /// <param name="ClearTariffsResult2">Another ClearTariffsResult.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ClearTariffsResult ClearTariffsResult1,
                                           ClearTariffsResult ClearTariffsResult2)

            => !(ClearTariffsResult1 == ClearTariffsResult2);

        #endregion

        #region Operator <  (ClearTariffsResult1, ClearTariffsResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearTariffsResult1">An ClearTariffsResult.</param>
        /// <param name="ClearTariffsResult2">Another ClearTariffsResult.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ClearTariffsResult ClearTariffsResult1,
                                          ClearTariffsResult ClearTariffsResult2)

            => ClearTariffsResult1 is null
                   ? throw new ArgumentNullException(nameof(ClearTariffsResult1), "The given ClearTariffsResult must not be null!")
                   : ClearTariffsResult1.CompareTo(ClearTariffsResult2) < 0;

        #endregion

        #region Operator <= (ClearTariffsResult1, ClearTariffsResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearTariffsResult1">An ClearTariffsResult.</param>
        /// <param name="ClearTariffsResult2">Another ClearTariffsResult.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ClearTariffsResult ClearTariffsResult1,
                                           ClearTariffsResult ClearTariffsResult2)

            => !(ClearTariffsResult1 > ClearTariffsResult2);

        #endregion

        #region Operator >  (ClearTariffsResult1, ClearTariffsResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearTariffsResult1">An ClearTariffsResult.</param>
        /// <param name="ClearTariffsResult2">Another ClearTariffsResult.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ClearTariffsResult ClearTariffsResult1,
                                          ClearTariffsResult ClearTariffsResult2)

            => ClearTariffsResult1 is null
                   ? throw new ArgumentNullException(nameof(ClearTariffsResult1), "The given ClearTariffsResult must not be null!")
                   : ClearTariffsResult1.CompareTo(ClearTariffsResult2) > 0;

        #endregion

        #region Operator >= (ClearTariffsResult1, ClearTariffsResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearTariffsResult1">An ClearTariffsResult.</param>
        /// <param name="ClearTariffsResult2">Another ClearTariffsResult.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ClearTariffsResult ClearTariffsResult1,
                                           ClearTariffsResult ClearTariffsResult2)

            => !(ClearTariffsResult1 < ClearTariffsResult2);

        #endregion

        #endregion

        #region IComparable<ClearTariffsResult> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two ClearTariffsResults.
        /// </summary>
        /// <param name="Object">An ClearTariffsResult to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ClearTariffsResult clearTariffsResult
                   ? CompareTo(clearTariffsResult)
                   : throw new ArgumentException("The given object is not a ClearTariffsResult!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ClearTariffsResult)

        /// <summary>
        /// Compares two ClearTariffsResults.
        /// </summary>
        /// <param name="ClearTariffsResult">An ClearTariffsResult to compare with.</param>
        public Int32 CompareTo(ClearTariffsResult? ClearTariffsResult)
        {

            if (ClearTariffsResult is null)
                throw new ArgumentNullException(nameof(ClearTariffsResult), "The given ClearTariffsResult must not be null!");

            var c = TariffId.CompareTo(ClearTariffsResult.TariffId);

            if (c == 0)
                c = Status.  CompareTo(ClearTariffsResult.Status);

            //if (c == 0 && StatusInfo is not null && ClearTariffsResult.StatusInfo is not null)
            //    c = StatusInfo.CompareTo(ClearTariffsResult.StatusInfo);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ClearTariffsResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearTariffsResults for equality.
        /// </summary>
        /// <param name="Object">An ClearTariffsResult to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearTariffsResult clearTariffsResult &&
                   Equals(clearTariffsResult);

        #endregion

        #region Equals(ClearTariffsResult)

        /// <summary>
        /// Compares two ClearTariffsResults for equality.
        /// </summary>
        /// <param name="ClearTariffsResult">An ClearTariffsResult to compare with.</param>
        public Boolean Equals(ClearTariffsResult? ClearTariffsResult)

            => ClearTariffsResult is not null &&

               TariffId.Equals(ClearTariffsResult.TariffId) &&
               Status.  Equals(ClearTariffsResult.Status)   &&

             ((StatusInfo is     null && ClearTariffsResult.StatusInfo is     null)  ||
              (StatusInfo is not null && ClearTariffsResult.StatusInfo is not null  && StatusInfo.Equals(ClearTariffsResult.StatusInfo)));

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

            => $"{TariffId} ({Status}){(StatusInfo is not null ? $", '{StatusInfo}'" : "")}";

        #endregion

    }

}
