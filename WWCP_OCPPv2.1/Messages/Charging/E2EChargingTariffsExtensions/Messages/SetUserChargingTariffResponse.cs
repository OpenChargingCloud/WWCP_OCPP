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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    //Note: This command is a draft version of the OCPP 2.1 specification
    //       and might be subject to change in future versions of the specification!

    /// <summary>
    /// A set user charging tariff response.
    /// </summary>
    public class SetUserChargingTariffResponse : AResponse<SetUserChargingTariffRequest,
                                                           SetUserChargingTariffResponse>,
                                                 IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/setUserChargingTariffResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The generic status.
        /// </summary>
        [Mandatory]
        public GenericStatus  Status        { get; }

        /// <summary>
        /// An optional element providing more information about the status.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new set user charging tariff response.
        /// </summary>
        /// <param name="Request">The set user charging tariff request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public SetUserChargingTariffResponse(CSMS.SetUserChargingTariffRequest  Request,
                                             GenericStatus                      Status,
                                             StatusInfo?                        StatusInfo            = null,

                                             Result?                            Result                = null,
                                             DateTime?                          ResponseTimestamp     = null,

                                             SourceRouting?                     Destination           = null,
                                             NetworkPath?                       NetworkPath           = null,

                                             IEnumerable<KeyPair>?              SignKeys              = null,
                                             IEnumerable<SignInfo>?             SignInfos             = null,
                                             IEnumerable<Signature>?            Signatures            = null,

                                             CustomData?                        CustomData            = null,

                                             SerializationFormats?              SerializationFormat   = null,
                                             CancellationToken                  CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.Status             = Status;
            this.StatusInfo         = StatusInfo;

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

        #region (static) Parse   (Request, JSON, CustomSetUserChargingTariffResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a set user charging tariff response.
        /// </summary>
        /// <param name="Request">The set user charging tariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetUserChargingTariffResponseParser">A delegate to parse custom set user charging tariff responses.</param>
        public static SetUserChargingTariffResponse Parse(CSMS.SetUserChargingTariffRequest                            Request,
                                                          JObject                                                      JSON,
                                                          SourceRouting                                            Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    ResponseTimestamp                           = null,
                                                          CustomJObjectParserDelegate<SetUserChargingTariffResponse>?  CustomSetUserChargingTariffResponseParser   = null,
                                                          CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {


            if (TryParse(Request,
                         JSON,
                     Destination,
                         NetworkPath,
                         out var setUserChargingTariffResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSetUserChargingTariffResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser) &&
                setUserChargingTariffResponse is not null)
            {
                return setUserChargingTariffResponse;
            }

            throw new ArgumentException("The given JSON representation of a set user charging tariff response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetUserChargingTariffResponse, out ErrorResponse, CustomSetUserChargingTariffResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a set user charging tariff response.
        /// </summary>
        /// <param name="Request">The set user charging tariff request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetUserChargingTariffResponse">The parsed set user charging tariff response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetUserChargingTariffResponseParser">A delegate to parse custom set user charging tariff responses.</param>
        public static Boolean TryParse(CSMS.SetUserChargingTariffRequest                            Request,
                                       JObject                                                      JSON,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                                  NetworkPath,
                                       out SetUserChargingTariffResponse?                           SetUserChargingTariffResponse,
                                       out String?                                                  ErrorResponse,
                                       DateTime?                                                    ResponseTimestamp                           = null,
                                       CustomJObjectParserDelegate<SetUserChargingTariffResponse>?  CustomSetUserChargingTariffResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                SetUserChargingTariffResponse = null;

                #region Status               [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "set user charging tariff status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo           [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

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


                SetUserChargingTariffResponse = new SetUserChargingTariffResponse(

                                                    Request,
                                                    Status,
                                                    StatusInfo,

                                                    null,
                                                    ResponseTimestamp,

                                                Destination,
                                                    NetworkPath,

                                                    null,
                                                    null,
                                                    Signatures,

                                                    CustomData

                                                );

                if (CustomSetUserChargingTariffResponseParser is not null)
                    SetUserChargingTariffResponse = CustomSetUserChargingTariffResponseParser(JSON,
                                                                                              SetUserChargingTariffResponse);

                return true;

            }
            catch (Exception e)
            {
                SetUserChargingTariffResponse  = null;
                ErrorResponse                  = "The given JSON representation of a set user charging tariff response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetUserChargingTariffResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetUserChargingTariffResponseSerializer">A delegate to serialize custom set user charging tariff responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetUserChargingTariffResponse>?  CustomSetUserChargingTariffResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                     CustomStatusInfoSerializer                      = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray (Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetUserChargingTariffResponseSerializer is not null
                       ? CustomSetUserChargingTariffResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SetUserChargingTariffResponse failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request.</param>
        public static SetUserChargingTariffResponse RequestError(CSMS.SetUserChargingTariffRequest  Request,
                                                                 EventTracking_Id                   EventTrackingId,
                                                                 ResultCode                         ErrorCode,
                                                                 String?                            ErrorDescription    = null,
                                                                 JObject?                           ErrorDetails        = null,
                                                                 DateTime?                          ResponseTimestamp   = null,

                                                                 SourceRouting?                     Destination         = null,
                                                                 NetworkPath?                       NetworkPath         = null,

                                                                 IEnumerable<KeyPair>?              SignKeys            = null,
                                                                 IEnumerable<SignInfo>?             SignInfos           = null,
                                                                 IEnumerable<Signature>?            Signatures          = null,

                                                                 CustomData?                        CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
                   null,
                  OCPPv2_1.Result.FromErrorResponse(
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
        /// The GetDefaultChargingTariff failed.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetUserChargingTariffResponse FormationViolation(CSMS.SetUserChargingTariffRequest  Request,
                                                                       String                             ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetDefaultChargingTariff failed.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetUserChargingTariffResponse SignatureError(CSMS.SetUserChargingTariffRequest  Request,
                                                                   String                             ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetDefaultChargingTariff failed.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SetUserChargingTariffResponse Failed(CSMS.SetUserChargingTariffRequest  Request,
                                                           String?                            Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The GetDefaultChargingTariff failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetDefaultChargingTariff request.</param>
        /// <param name="Exception">The exception.</param>
        public static SetUserChargingTariffResponse ExceptionOccurred(CSMS.SetUserChargingTariffRequest  Request,
                                                                     Exception                          Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SetUserChargingTariffResponse1, SetUserChargingTariffResponse2)

        /// <summary>
        /// Compares two set user charging tariff responses for equality.
        /// </summary>
        /// <param name="SetUserChargingTariffResponse1">A set user charging tariff response.</param>
        /// <param name="SetUserChargingTariffResponse2">Another set user charging tariff response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetUserChargingTariffResponse? SetUserChargingTariffResponse1,
                                           SetUserChargingTariffResponse? SetUserChargingTariffResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetUserChargingTariffResponse1, SetUserChargingTariffResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetUserChargingTariffResponse1 is null || SetUserChargingTariffResponse2 is null)
                return false;

            return SetUserChargingTariffResponse1.Equals(SetUserChargingTariffResponse2);

        }

        #endregion

        #region Operator != (SetUserChargingTariffResponse1, SetUserChargingTariffResponse2)

        /// <summary>
        /// Compares two set user charging tariff responses for inequality.
        /// </summary>
        /// <param name="SetUserChargingTariffResponse1">A set user charging tariff response.</param>
        /// <param name="SetUserChargingTariffResponse2">Another set user charging tariff response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetUserChargingTariffResponse? SetUserChargingTariffResponse1,
                                           SetUserChargingTariffResponse? SetUserChargingTariffResponse2)

            => !(SetUserChargingTariffResponse1 == SetUserChargingTariffResponse2);

        #endregion

        #endregion

        #region IEquatable<SetUserChargingTariffResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two set user charging tariff responses for equality.
        /// </summary>
        /// <param name="Object">A set user charging tariff response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetUserChargingTariffResponse setUserChargingTariffResponse &&
                   Equals(setUserChargingTariffResponse);

        #endregion

        #region Equals(SetUserChargingTariffResponse)

        /// <summary>
        /// Compares two set user charging tariff responses for equality.
        /// </summary>
        /// <param name="SetUserChargingTariffResponse">A set user charging tariff response to compare with.</param>
        public override Boolean Equals(SetUserChargingTariffResponse? SetUserChargingTariffResponse)

            => SetUserChargingTariffResponse is not null &&

               Status.Equals(SetUserChargingTariffResponse.Status) &&

             ((StatusInfo is     null && SetUserChargingTariffResponse.StatusInfo is     null) ||
              (StatusInfo is not null && SetUserChargingTariffResponse.StatusInfo is not null && StatusInfo.Equals(SetUserChargingTariffResponse.StatusInfo))) &&

               base.GenericEquals(SetUserChargingTariffResponse);

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

            => $"{Status.AsText()}{(StatusInfo is not null ? $", {StatusInfo}" : "")}";

        #endregion

    }

}
