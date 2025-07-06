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
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A TimeTravel response.
    /// </summary>
    public class TimeTravelResponse : AResponse<TimeTravelRequest,
                                                TimeTravelResponse>,
                                      IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/timeTravelResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The response status.
        /// </summary>
        public GenericStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new TimeTravel response.
        /// </summary>
        /// <param name="Request">The TimeTravel request leading to this response.</param>
        /// <param name="Status">The response status.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public TimeTravelResponse(TimeTravelRequest        Request,
                                  GenericStatus            Status,
                                  StatusInfo?              StatusInfo            = null,

                                  Result?                  Result                = null,
                                  DateTime?                ResponseTimestamp     = null,

                                  SourceRouting?           Destination           = null,
                                  NetworkPath?             NetworkPath           = null,

                                  IEnumerable<KeyPair>?    SignKeys              = null,
                                  IEnumerable<SignInfo>?   SignInfos             = null,
                                  IEnumerable<Signature>?  Signatures            = null,

                                  CustomData?              CustomData            = null,

                                  SerializationFormats?    SerializationFormat   = null,
                                  CancellationToken        CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat,
                   CancellationToken)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a TimeTravel response.
        /// </summary>
        /// <param name="Request">The TimeTravel request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomTimeTravelResponseParser">An optional delegate to parse custom TimeTravel responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static TimeTravelResponse Parse(TimeTravelRequest                                 Request,
                                               JObject                                           JSON,
                                               SourceRouting                                     Destination,
                                               NetworkPath                                       NetworkPath,
                                               DateTime?                                         ResponseTimestamp                = null,
                                               CustomJObjectParserDelegate<TimeTravelResponse>?  CustomTimeTravelResponseParser   = null,
                                               CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                               CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var timeTravelResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomTimeTravelResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return timeTravelResponse;
            }

            throw new ArgumentException("The given JSON representation of a TimeTravel response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out TimeTravelResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a TimeTravel response.
        /// </summary>
        /// <param name="Request">The TimeTravel request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="TimeTravelResponse">The parsed TimeTravel response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomTimeTravelResponseParser">An optional delegate to parse custom TimeTravel responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(TimeTravelRequest                                 Request,
                                       JObject                                           JSON,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out TimeTravelResponse?      TimeTravelResponse,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTime?                                         ResponseTimestamp                = null,
                                       CustomJObjectParserDelegate<TimeTravelResponse>?  CustomTimeTravelResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                       CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            try
            {

                TimeTravelResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic status",
                                         GenericStatus.TryParse,
                                         out GenericStatus status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo     [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? statusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures     [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData     [optional]

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


                TimeTravelResponse = new TimeTravelResponse(

                                         Request,
                                         status,
                                         statusInfo,

                                         null,
                                         ResponseTimestamp,

                                         Destination,
                                         NetworkPath,

                                         null,
                                         null,
                                         Signatures,

                                         CustomData

                                     );

                if (CustomTimeTravelResponseParser is not null)
                    TimeTravelResponse = CustomTimeTravelResponseParser(JSON,
                                                                        TimeTravelResponse);

                return true;

            }
            catch (Exception e)
            {
                TimeTravelResponse  = null;
                ErrorResponse          = "The given JSON representation of a TimeTravel response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTimeTravelResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTimeTravelResponseSerializer">A delegate to serialize custom TimeTravel responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TimeTravelResponse>?  CustomTimeTravelResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?          CustomStatusInfoSerializer           = null,
                              CustomJObjectSerializerDelegate<Signature>?           CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.ToString()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomTimeTravelResponseSerializer is not null
                       ? CustomTimeTravelResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The TimeTravel failed because of a request error.
        /// </summary>
        /// <param name="Request">The TimeTravel request leading to this response.</param>
        public static TimeTravelResponse RequestError(TimeTravelRequest        Request,
                                                      EventTracking_Id         EventTrackingId,
                                                      ResultCode               ErrorCode,
                                                      String?                  ErrorDescription    = null,
                                                      JObject?                 ErrorDetails        = null,
                                                      DateTime?                ResponseTimestamp   = null,

                                                      SourceRouting?           Destination         = null,
                                                      NetworkPath?             NetworkPath         = null,

                                                      IEnumerable<KeyPair>?    SignKeys            = null,
                                                      IEnumerable<SignInfo>?   SignInfos           = null,
                                                      IEnumerable<Signature>?  Signatures          = null,

                                                      CustomData?              CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
                   null,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The TimeTravel failed.
        /// </summary>
        /// <param name="Request">The TimeTravel request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static TimeTravelResponse FormationViolation(TimeTravelRequest  Request,
                                                            String             ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FormationViolation(
                                              $"Invalid data format: {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The TimeTravel failed.
        /// </summary>
        /// <param name="Request">The TimeTravel request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static TimeTravelResponse SignatureError(TimeTravelRequest  Request,
                                                        String             ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.SignatureError(
                                              $"Invalid signature(s): {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The TimeTravel failed.
        /// </summary>
        /// <param name="Request">The TimeTravel request.</param>
        /// <param name="Description">An optional error description.</param>
        public static TimeTravelResponse Failed(TimeTravelRequest  Request,
                                                String?            Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.Server(Description),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The TimeTravel failed because of an exception.
        /// </summary>
        /// <param name="Request">The TimeTravel request.</param>
        /// <param name="Exception">The exception.</param>
        public static TimeTravelResponse ExceptionOccurred(TimeTravelRequest  Request,
                                                           Exception          Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FromException(Exception),
                    SerializationFormat:  Request.SerializationFormat);

        #endregion


        #region Operator overloading

        #region Operator == (TimeTravelResponse1, TimeTravelResponse2)

        /// <summary>
        /// Compares two TimeTravel responses for equality.
        /// </summary>
        /// <param name="TimeTravelResponse1">A TimeTravel response.</param>
        /// <param name="TimeTravelResponse2">Another TimeTravel response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TimeTravelResponse? TimeTravelResponse1,
                                           TimeTravelResponse? TimeTravelResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TimeTravelResponse1, TimeTravelResponse2))
                return true;

            // If one is null, but not both, return false.
            if (TimeTravelResponse1 is null || TimeTravelResponse2 is null)
                return false;

            return TimeTravelResponse1.Equals(TimeTravelResponse2);

        }

        #endregion

        #region Operator != (TimeTravelResponse1, TimeTravelResponse2)

        /// <summary>
        /// Compares two TimeTravel responses for inequality.
        /// </summary>
        /// <param name="TimeTravelResponse1">A TimeTravel response.</param>
        /// <param name="TimeTravelResponse2">Another TimeTravel response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TimeTravelResponse? TimeTravelResponse1,
                                           TimeTravelResponse? TimeTravelResponse2)

            => !(TimeTravelResponse1 == TimeTravelResponse2);

        #endregion

        #endregion

        #region IEquatable<TimeTravelResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TimeTravel responses for equality.
        /// </summary>
        /// <param name="Object">A TimeTravel response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TimeTravelResponse timeTravelResponse &&
                   Equals(timeTravelResponse);

        #endregion

        #region Equals(TimeTravelResponse)

        /// <summary>
        /// Compares two TimeTravel responses for equality.
        /// </summary>
        /// <param name="TimeTravelResponse">A TimeTravel response to compare with.</param>
        public override Boolean Equals(TimeTravelResponse? TimeTravelResponse)

            => TimeTravelResponse is not null &&

               Status.     Equals(TimeTravelResponse.Status) &&

             ((StatusInfo is     null && TimeTravelResponse.StatusInfo is     null) ||
               StatusInfo is not null && TimeTravelResponse.StatusInfo is not null && StatusInfo.Equals(TimeTravelResponse.StatusInfo)) &&

               base.GenericEquals(TimeTravelResponse);

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

            => String.Concat(

                   Status,

                   StatusInfo is not null
                       ? $", statusInfo: '{StatusInfo}'"
                       : ""

               );

        #endregion

    }

}
