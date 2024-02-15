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

using System.Collections.ObjectModel;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    //Note: This command is a draft version of the OCPP 2.1 specification
    //       and might be subject to change in future versions of the specification!

    /// <summary>
    /// A set user charging tariff response.
    /// </summary>
    public class SetUserChargingTariffResponse : AResponse<CSMS.SetUserChargingTariffRequest,
                                                                SetUserChargingTariffResponse>,
                                                 IResponse
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

        #region SetUserChargingTariffResponse(Request, Status, StatusInfo = null, ...)

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
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SetUserChargingTariffResponse(CSMS.SetUserChargingTariffRequest  Request,
                                             GenericStatus                      Status,
                                             StatusInfo?                        StatusInfo          = null,
                                             DateTime?                          ResponseTimestamp   = null,

                                             IEnumerable<KeyPair>?              SignKeys            = null,
                                             IEnumerable<SignInfo>?             SignInfos           = null,
                                             IEnumerable<OCPP.Signature>?       Signatures          = null,

                                             CustomData?                        CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

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

        #region SetUserChargingTariffResponse(Request, Result)

        /// <summary>
        /// Create a new set user charging tariff response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public SetUserChargingTariffResponse(CSMS.SetUserChargingTariffRequest  Request,
                                             Result                             Result)

            : base(Request,
                   Result)

        {

            this.Status  = GenericStatus.Rejected;


            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.            GetHashCode();

            }

        }

        #endregion

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
                                                          CustomJObjectParserDelegate<SetUserChargingTariffResponse>?  CustomSetUserChargingTariffResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var setUserChargingTariffResponse,
                         out var errorResponse,
                         CustomSetUserChargingTariffResponseParser) &&
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
                                       out SetUserChargingTariffResponse?                           SetUserChargingTariffResponse,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<SetUserChargingTariffResponse>?  CustomSetUserChargingTariffResponseParser   = null)
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
                                           OCPP.StatusInfo.TryParse,
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
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
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
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                 CustomSignatureSerializer                       = null,
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
        /// The set user charging tariff failed.
        /// </summary>
        public static SetUserChargingTariffResponse Failed(CSMS.SetUserChargingTariffRequest Request)

            => new (Request,
                    Result.Server());

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
