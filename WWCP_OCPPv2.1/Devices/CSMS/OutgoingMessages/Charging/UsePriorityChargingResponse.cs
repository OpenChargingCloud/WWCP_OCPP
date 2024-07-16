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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// An use priority charging response.
    /// </summary>
    public class UsePriorityChargingResponse : AResponse<CSMS.UsePriorityChargingRequest,
                                                              UsePriorityChargingResponse>,
                                               IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/usePriorityChargingResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the use priority chargingl request.
        /// </summary>
        public GenericStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region UsePriorityChargingResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new use priority charging response.
        /// </summary>
        /// <param name="Request">The use priority charging request leading to this response.</param>
        /// <param name="Status">The success or failure of the use priority charging request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public UsePriorityChargingResponse(CSMS.UsePriorityChargingRequest  Request,
                                           GenericStatus                    Status,
                                           StatusInfo?                      StatusInfo          = null,
                                           DateTime?                        ResponseTimestamp   = null,

                                           IEnumerable<KeyPair>?            SignKeys            = null,
                                           IEnumerable<SignInfo>?           SignInfos           = null,
                                           IEnumerable<Signature>?          Signatures          = null,

                                           CustomData?                      CustomData          = null)

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

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region UsePriorityChargingResponse(Result)

        /// <summary>
        /// Create a new use priority charging response.
        /// </summary>
        /// <param name="Request">The use priority charging request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public UsePriorityChargingResponse(CSMS.UsePriorityChargingRequest  Request,
                                           Result                           Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomUsePriorityChargingResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an use priority charging response.
        /// </summary>
        /// <param name="Request">The use priority charging request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomUsePriorityChargingResponseParser">A delegate to parse custom use priority charging responses.</param>
        public static UsePriorityChargingResponse Parse(CSMS.UsePriorityChargingRequest                            Request,
                                                        JObject                                                    JSON,
                                                        CustomJObjectParserDelegate<UsePriorityChargingResponse>?  CustomUsePriorityChargingResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var usePriorityChargingResponse,
                         out var errorResponse,
                         CustomUsePriorityChargingResponseParser))
            {
                return usePriorityChargingResponse;
            }

            throw new ArgumentException("The given JSON representation of an use priority charging response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out UsePriorityChargingResponse, out ErrorResponse, CustomUsePriorityChargingResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an use priority charging response.
        /// </summary>
        /// <param name="Request">The use priority charging request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UsePriorityChargingResponse">The parsed use priority charging response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUsePriorityChargingResponseParser">A delegate to parse custom use priority charging responses.</param>
        public static Boolean TryParse(CSMS.UsePriorityChargingRequest                            Request,
                                       JObject                                                    JSON,
                                       [NotNullWhen(true)]  out UsePriorityChargingResponse?      UsePriorityChargingResponse,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       CustomJObjectParserDelegate<UsePriorityChargingResponse>?  CustomUsePriorityChargingResponseParser   = null)
        {

            try
            {

                UsePriorityChargingResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

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

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                UsePriorityChargingResponse = new UsePriorityChargingResponse(
                                                  Request,
                                                  Status,
                                                  StatusInfo,
                                                  null,
                                                  null,
                                                  null,
                                                  Signatures,
                                                  CustomData
                                              );

                if (CustomUsePriorityChargingResponseParser is not null)
                    UsePriorityChargingResponse = CustomUsePriorityChargingResponseParser(JSON,
                                                                                          UsePriorityChargingResponse);

                return true;

            }
            catch (Exception e)
            {
                UsePriorityChargingResponse  = null;
                ErrorResponse                = "The given JSON representation of an use priority charging response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUsePriorityChargingResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUsePriorityChargingResponseSerializer">A delegate to serialize custom use priority charging responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UsePriorityChargingResponse>?  CustomUsePriorityChargingResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                   CustomStatusInfoSerializer                    = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUsePriorityChargingResponseSerializer is not null
                       ? CustomUsePriorityChargingResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The use priority charging command failed.
        /// </summary>
        /// <param name="Request">The use priority charging request leading to this response.</param>
        public static UsePriorityChargingResponse Failed(CSMS.UsePriorityChargingRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (UsePriorityChargingResponse1, UsePriorityChargingResponse2)

        /// <summary>
        /// Compares two use priority charging responses for equality.
        /// </summary>
        /// <param name="UsePriorityChargingResponse1">An use priority charging response.</param>
        /// <param name="UsePriorityChargingResponse2">Another use priority charging response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UsePriorityChargingResponse? UsePriorityChargingResponse1,
                                           UsePriorityChargingResponse? UsePriorityChargingResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UsePriorityChargingResponse1, UsePriorityChargingResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UsePriorityChargingResponse1 is null || UsePriorityChargingResponse2 is null)
                return false;

            return UsePriorityChargingResponse1.Equals(UsePriorityChargingResponse2);

        }

        #endregion

        #region Operator != (UsePriorityChargingResponse1, UsePriorityChargingResponse2)

        /// <summary>
        /// Compares two use priority charging responses for inequality.
        /// </summary>
        /// <param name="UsePriorityChargingResponse1">An use priority charging response.</param>
        /// <param name="UsePriorityChargingResponse2">Another use priority charging response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UsePriorityChargingResponse? UsePriorityChargingResponse1,
                                           UsePriorityChargingResponse? UsePriorityChargingResponse2)

            => !(UsePriorityChargingResponse1 == UsePriorityChargingResponse2);

        #endregion

        #endregion

        #region IEquatable<UsePriorityChargingResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two use priority charging responses for equality.
        /// </summary>
        /// <param name="UsePriorityChargingResponse">An use priority charging response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UsePriorityChargingResponse usePriorityChargingResponse &&
                   Equals(usePriorityChargingResponse);

        #endregion

        #region Equals(UsePriorityChargingResponse)

        /// <summary>
        /// Compares two use priority charging responses for equality.
        /// </summary>
        /// <param name="UsePriorityChargingResponse">An use priority charging response to compare with.</param>
        public override Boolean Equals(UsePriorityChargingResponse? UsePriorityChargingResponse)

            => UsePriorityChargingResponse is not null &&

               Status.Equals(UsePriorityChargingResponse.Status) &&

             ((StatusInfo is     null && UsePriorityChargingResponse.StatusInfo is     null) ||
               StatusInfo is not null && UsePriorityChargingResponse.StatusInfo is not null && StatusInfo.Equals(UsePriorityChargingResponse.StatusInfo)) &&

               base.GenericEquals(UsePriorityChargingResponse);

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

                return Status.     GetHashCode()       * 5 ^
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

            => Status.ToString();

        #endregion

    }

}
