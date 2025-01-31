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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Shows assignments of tariffs to EVSEs or IdTokens.
    /// </summary>
    public class TariffAssignment : IEquatable<TariffAssignment>,
                                    IComparable<TariffAssignment>,
                                    IComparable
    {

        #region Properties

        /// <summary>
        /// The tariff identification.
        /// </summary>
        [Mandatory]
        public Tariff_Id             TariffId      { get; }

        /// <summary>
        /// The tariff kind (user|default).
        /// </summary>
        [Mandatory]
        public TariffKind            TariffKind    { get; }

        /// <summary>
        /// The optional enumeration of EVSE identifications this tariff applies to.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE_Id>  EVSEIds       { get; }

        /// <summary>
        /// The optional enumeration of IdTokens this tariff applies to.
        /// </summary>
        [Optional]
        public IEnumerable<IdToken>  IdTokens      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new assignments of tariffs to EVSEs or IdTokens.
        /// </summary>
        /// <param name="TariffId">The tariff identification.</param>
        /// <param name="TariffKind">The tariff kind (user|default).</param>
        /// <param name="EVSEIds">An optional enumeration of EVSE identifications this tariff applies to.</param>
        /// <param name="IdTokens">An optional enumeration of IdTokens this tariff applies to.</param>
        public TariffAssignment(Tariff_Id              TariffId,
                                TariffKind             TariffKind,
                                IEnumerable<EVSE_Id>?  EVSEIds    = null,
                                IEnumerable<IdToken>?  IdTokens   = null)
        {

            this.TariffId    = TariffId;
            this.TariffKind  = TariffKind;
            this.EVSEIds     = EVSEIds?. Distinct() ?? [];
            this.IdTokens    = IdTokens?.Distinct() ?? [];

            unchecked
            {

                hashCode = this.TariffId.  GetHashCode()  * 7 ^
                           this.TariffKind.GetHashCode()  * 5 ^
                           this.EVSEIds.   CalcHashCode() * 3 ^
                           this.IdTokens.  CalcHashCode();

            }

        }

        #endregion


        #region (static) Parse    (JSON, CustomTariffAssignmentParser = null)

        /// <summary>
        /// Parse the given JSON representation of a tariff assignment.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTariffAssignmentParser">An optional delegate to parse custom tariff assignment JSON objects.</param>
        public static TariffAssignment Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<TariffAssignment>?  CustomTariffAssignmentParser   = null)
        {

            if (TryParse(JSON,
                         out var tariffAssignment,
                         out var errorResponse,
                         CustomTariffAssignmentParser))
            {
                return tariffAssignment;
            }

            throw new ArgumentException("The given JSON representation of a tariff assignment is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse (JSON, out TariffAssignment, out ErrorResponse, CustomTariffAssignmentParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a tariff assignment.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffAssignment">The parsed tariff assignment.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out TariffAssignment?  TariffAssignment,
                                       [NotNullWhen(false)] out String?            ErrorResponse)

            => TryParse(JSON,
                        out TariffAssignment,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a tariff assignment.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffAssignment">The parsed tariff assignment.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffAssignmentParser">An optional delegate to parse custom tariff assignment JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       [NotNullWhen(true)]  out TariffAssignment?      TariffAssignment,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       CustomJObjectParserDelegate<TariffAssignment>?  CustomTariffAssignmentParser)
        {

            try
            {

                TariffAssignment = null;

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

                #region Parse TariffKind    [mandatory]

                if (!JSON.ParseMandatory("tariffKind",
                                         "EVSE identifications",
                                         OCPPv2_1.TariffKind.TryParse,
                                         out TariffKind TariffKind,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEIds       [optional]

                if (!JSON.ParseOptionalHashSet("evseIds",
                                               "EVSE identifications",
                                               EVSE_Id.TryParse,
                                               out HashSet<EVSE_Id> EVSEIds,
                                               out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse IdTokens      [optional]

                if (JSON.ParseOptionalHashSet("idTokens",
                                              "identification tokens",
                                              IdToken.TryParse,
                                              out HashSet<IdToken> IdTokens,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                TariffAssignment = new TariffAssignment(
                                       TariffId,
                                       TariffKind,
                                       EVSEIds,
                                       IdTokens
                                   );


                if (CustomTariffAssignmentParser is not null)
                    TariffAssignment = CustomTariffAssignmentParser(JSON,
                                                      TariffAssignment);

                return true;

            }
            catch (Exception e)
            {
                TariffAssignment  = default;
                ErrorResponse     = "The given JSON representation of a tariff assignment is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffAssignmentSerializer = null, CustomIdTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffAssignmentSerializer">A delegate to serialize custom tariff assignment JSON objects.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TariffAssignment>? CustomTariffAssignmentSerializer = null,
                              CustomJObjectSerializerDelegate<IdToken>?          CustomIdTokenSerializer          = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?   CustomAdditionalInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?       CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("tariffId",     TariffId.  ToString()),
                                 new JProperty("tariffKind",   TariffKind.ToString()),

                           EVSEIds.Any()
                               ? new JProperty("evseIds",      new JArray(EVSEIds. Select(evseId  => evseId. ToString())))
                               : null,

                           IdTokens.Any()
                               ? new JProperty("idTokens",     new JArray(IdTokens.Select(idToken => idToken.ToJSON(CustomIdTokenSerializer,
                                                                                                                    CustomAdditionalInfoSerializer,
                                                                                                                    CustomCustomDataSerializer))))
                               : null

                       );

            return CustomTariffAssignmentSerializer is not null
                       ? CustomTariffAssignmentSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this tariff assignment.
        /// </summary>
        public TariffAssignment Clone()

            => new (
                   TariffId.  Clone(),
                   TariffKind.Clone(),
                   EVSEIds.   Select(evseId  => evseId. Clone()),
                   IdTokens.  Select(idToken => idToken.Clone())
               );

        #endregion


        #region Operator overloading

        #region Operator == (TariffAssignment1, TariffAssignment2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssignment1">An tariff assignment.</param>
        /// <param name="TariffAssignment2">Another tariff assignment.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffAssignment TariffAssignment1,
                                           TariffAssignment TariffAssignment2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TariffAssignment1, TariffAssignment2))
                return true;

            // If one is null, but not both, return false.
            if (TariffAssignment1 is null || TariffAssignment2 is null)
                return false;

            return TariffAssignment1.Equals(TariffAssignment2);

        }

        #endregion

        #region Operator != (TariffAssignment1, TariffAssignment2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssignment1">An tariff assignment.</param>
        /// <param name="TariffAssignment2">Another tariff assignment.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffAssignment TariffAssignment1,
                                           TariffAssignment TariffAssignment2)

            => !(TariffAssignment1 == TariffAssignment2);

        #endregion

        #region Operator <  (TariffAssignment1, TariffAssignment2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssignment1">An tariff assignment.</param>
        /// <param name="TariffAssignment2">Another tariff assignment.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (TariffAssignment TariffAssignment1,
                                          TariffAssignment TariffAssignment2)

            => TariffAssignment1 is null
                   ? throw new ArgumentNullException(nameof(TariffAssignment1), "The given tariff assignment must not be null!")
                   : TariffAssignment1.CompareTo(TariffAssignment2) < 0;

        #endregion

        #region Operator <= (TariffAssignment1, TariffAssignment2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssignment1">An tariff assignment.</param>
        /// <param name="TariffAssignment2">Another tariff assignment.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (TariffAssignment TariffAssignment1,
                                           TariffAssignment TariffAssignment2)

            => !(TariffAssignment1 > TariffAssignment2);

        #endregion

        #region Operator >  (TariffAssignment1, TariffAssignment2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssignment1">An tariff assignment.</param>
        /// <param name="TariffAssignment2">Another tariff assignment.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (TariffAssignment TariffAssignment1,
                                          TariffAssignment TariffAssignment2)

            => TariffAssignment1 is null
                   ? throw new ArgumentNullException(nameof(TariffAssignment1), "The given tariff assignment must not be null!")
                   : TariffAssignment1.CompareTo(TariffAssignment2) > 0;

        #endregion

        #region Operator >= (TariffAssignment1, TariffAssignment2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffAssignment1">An tariff assignment.</param>
        /// <param name="TariffAssignment2">Another tariff assignment.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (TariffAssignment TariffAssignment1,
                                           TariffAssignment TariffAssignment2)

            => !(TariffAssignment1 < TariffAssignment2);

        #endregion

        #endregion

        #region IComparable<TariffAssignment> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two tariff assignments.
        /// </summary>
        /// <param name="Object">An tariff assignment to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TariffAssignment tariffAssignment
                   ? CompareTo(tariffAssignment)
                   : throw new ArgumentException("The given object is not a tariff assignment!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TariffAssignment)

        /// <summary>
        /// Compares two tariff assignments.
        /// </summary>
        /// <param name="TariffAssignment">An tariff assignment to compare with.</param>
        public Int32 CompareTo(TariffAssignment? TariffAssignment)
        {

            if (TariffAssignment is null)
                throw new ArgumentNullException(nameof(TariffAssignment), "The given tariff assignment must not be null!");

            var c = TariffId.    CompareTo(TariffAssignment.TariffId);

            if (c == 0)
                c = TariffKind.  CompareTo(TariffAssignment.TariffKind);

            if (c == 0)
                c = EVSEIds.Count().CompareTo(TariffAssignment.EVSEIds.Count());

            if (c == 0)
            {

                var a =                  EVSEIds.OrderBy(evseId => evseId).ToArray();
                var b = TariffAssignment.EVSEIds.OrderBy(evseId => evseId).ToArray();

                for (var i = 0; i < a.Length; i++)
                {

                    c = a[i].CompareTo(b[i]);

                    if (c != 0)
                        return c;

                }

            }

            if (c == 0)
                c = IdTokens.Count().CompareTo(TariffAssignment.IdTokens.Count());

            if (c == 0)
            {

                var a =                  IdTokens.OrderBy(idToken => idToken).ToArray();
                var b = TariffAssignment.IdTokens.OrderBy(idToken => idToken).ToArray();

                for (var i = 0; i < a.Length; i++)
                {

                    c = a[i].CompareTo(b[i]);

                    if (c != 0)
                        return c;

                }

            }

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<TariffAssignment> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two tariff assignments for equality.
        /// </summary>
        /// <param name="Object">An tariff assignment to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffAssignment tariffAssignment &&
                   Equals(tariffAssignment);

        #endregion

        #region Equals(TariffAssignment)

        /// <summary>
        /// Compares two tariff assignments for equality.
        /// </summary>
        /// <param name="TariffAssignment">An tariff assignment to compare with.</param>
        public Boolean Equals(TariffAssignment? TariffAssignment)

            => TariffAssignment is not null &&

               TariffId.  Equals(TariffAssignment.TariffId)   &&
               TariffKind.Equals(TariffAssignment.TariffKind) &&

               EVSEIds. Count().Equals(TariffAssignment.EVSEIds. Count()) &&
               EVSEIds.All(TariffAssignment.EVSEIds.Contains) &&

               IdTokens.Count().Equals(TariffAssignment.IdTokens.Count()) &&
               IdTokens.All(TariffAssignment.IdTokens.Contains);

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

            => $"{TariffId} ({TariffKind}) => EVSE Ids: {EVSEIds.AggregateWith(", ")}, IdTokens: {IdTokens.AggregateWith(", ")}";

        #endregion

    }

}
