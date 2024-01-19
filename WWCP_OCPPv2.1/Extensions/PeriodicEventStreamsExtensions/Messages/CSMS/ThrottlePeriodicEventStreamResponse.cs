/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/GetChargingCloud/WWCP_OCPP>
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
    /// A ThrottlePeriodicEventStream response.
    /// </summary>
    public class ThrottlePeriodicEventStreamResponse : AResponse<CSMS.ThrottlePeriodicEventStreamRequest,
                                                                      ThrottlePeriodicEventStreamResponse>,
                                                       IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/throttlePeriodicEventStreamResponse");

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
        [Mandatory]
        public GenericStatus  Status        { get; }

        /// <summary>
        /// An optional element providing more information about the response status.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region ThrottlePeriodicEventStreamResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new open periodic event stream response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="Status">An optional response status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the response status.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ThrottlePeriodicEventStreamResponse(CSMS.ThrottlePeriodicEventStreamRequest  Request,
                                                   GenericStatus                            Status,
                                                   StatusInfo?                              StatusInfo          = null,
                                                   DateTime?                                ResponseTimestamp   = null,

                                                   IEnumerable<KeyPair>?                    SignKeys            = null,
                                                   IEnumerable<SignInfo>?                   SignInfos           = null,
                                                   IEnumerable<OCPP.Signature>?             Signatures          = null,

                                                   CustomData?                              CustomData          = null)

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

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.            GetHashCode();

            }

        }

        #endregion

        #region ThrottlePeriodicEventStreamResponse(Request, Result)

        /// <summary>
        /// Create a new open periodic event stream response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public ThrottlePeriodicEventStreamResponse(CSMS.ThrottlePeriodicEventStreamRequest  Request,
                                                   Result                                   Result)

            : base(Request,
                   Result)

        {

            this.Status = GenericStatus.Rejected;

        }

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomThrottlePeriodicEventStreamResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get periodic event streams response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomThrottlePeriodicEventStreamResponseParser">A delegate to parse custom open periodic event stream responses.</param>
        public static ThrottlePeriodicEventStreamResponse Parse(CSMS.ThrottlePeriodicEventStreamRequest                            Request,
                                                                JObject                                                            JSON,
                                                                CustomJObjectParserDelegate<ThrottlePeriodicEventStreamResponse>?  CustomThrottlePeriodicEventStreamResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var throttlePeriodicEventStreamResponse,
                         out var errorResponse,
                         CustomThrottlePeriodicEventStreamResponseParser) &&
                throttlePeriodicEventStreamResponse is not null)
            {
                return throttlePeriodicEventStreamResponse;
            }

            throw new ArgumentException("The given JSON representation of a get periodic event streams response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ThrottlePeriodicEventStreamResponse, out ErrorResponse, CustomThrottlePeriodicEventStreamResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get periodic event streams response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ThrottlePeriodicEventStreamResponse">The parsed open periodic event stream response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomThrottlePeriodicEventStreamResponseParser">A delegate to parse custom open periodic event stream responses.</param>
        public static Boolean TryParse(CSMS.ThrottlePeriodicEventStreamRequest                            Request,
                                       JObject                                                            JSON,
                                       [NotNullWhen(true)]  out ThrottlePeriodicEventStreamResponse?      ThrottlePeriodicEventStreamResponse,
                                       [NotNullWhen(false)] out String?                                   ErrorResponse,
                                       CustomJObjectParserDelegate<ThrottlePeriodicEventStreamResponse>?  CustomThrottlePeriodicEventStreamResponseParser   = null)
        {

            try
            {

                ThrottlePeriodicEventStreamResponse = null;

                #region Status        [mandatory]

                if (JSON.ParseMandatory("status",
                                        "response status",
                                        GenericStatusExtensions.TryParse,
                                        out GenericStatus Status,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StatusInfo    [optional]

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

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ThrottlePeriodicEventStreamResponse = new ThrottlePeriodicEventStreamResponse(

                                                          Request,
                                                          Status,
                                                          StatusInfo,
                                                          null,

                                                          null,
                                                          null,
                                                          Signatures,

                                                          CustomData

                                                      );

                if (CustomThrottlePeriodicEventStreamResponseParser is not null)
                    ThrottlePeriodicEventStreamResponse = CustomThrottlePeriodicEventStreamResponseParser(JSON,
                                                                                                          ThrottlePeriodicEventStreamResponse);

                return true;

            }
            catch (Exception e)
            {
                ThrottlePeriodicEventStreamResponse  = null;
                ErrorResponse                        = "The given JSON representation of a ThrottlePeriodicEventStreamResponse is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomThrottlePeriodicEventStreamResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomThrottlePeriodicEventStreamResponseSerializer">A delegate to serialize custom open periodic event stream responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ThrottlePeriodicEventStreamResponse>?  CustomThrottlePeriodicEventStreamResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                           CustomStatusInfoSerializer                            = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                       CustomSignatureSerializer                             = null,
                              CustomJObjectSerializerDelegate<CustomData>?                           CustomCustomDataSerializer                            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",        Status.           AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",    StatusInfo.       ToJSON(CustomStatusInfoSerializer,
                                                                                         CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.       ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomThrottlePeriodicEventStreamResponseSerializer is not null
                       ? CustomThrottlePeriodicEventStreamResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The open periodic event stream failed.
        /// </summary>
        public static ThrottlePeriodicEventStreamResponse Failed(CSMS.ThrottlePeriodicEventStreamRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ThrottlePeriodicEventStreamResponse1, ThrottlePeriodicEventStreamResponse2)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="ThrottlePeriodicEventStreamResponse1">A get periodic event streams response.</param>
        /// <param name="ThrottlePeriodicEventStreamResponse2">Another open periodic event stream response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ThrottlePeriodicEventStreamResponse? ThrottlePeriodicEventStreamResponse1,
                                           ThrottlePeriodicEventStreamResponse? ThrottlePeriodicEventStreamResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ThrottlePeriodicEventStreamResponse1, ThrottlePeriodicEventStreamResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ThrottlePeriodicEventStreamResponse1 is null || ThrottlePeriodicEventStreamResponse2 is null)
                return false;

            return ThrottlePeriodicEventStreamResponse1.Equals(ThrottlePeriodicEventStreamResponse2);

        }

        #endregion

        #region Operator != (ThrottlePeriodicEventStreamResponse1, ThrottlePeriodicEventStreamResponse2)

        /// <summary>
        /// Compares two open periodic event stream responses for inequality.
        /// </summary>
        /// <param name="ThrottlePeriodicEventStreamResponse1">A get periodic event streams response.</param>
        /// <param name="ThrottlePeriodicEventStreamResponse2">Another open periodic event stream response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ThrottlePeriodicEventStreamResponse? ThrottlePeriodicEventStreamResponse1,
                                           ThrottlePeriodicEventStreamResponse? ThrottlePeriodicEventStreamResponse2)

            => !(ThrottlePeriodicEventStreamResponse1 == ThrottlePeriodicEventStreamResponse2);

        #endregion

        #endregion

        #region IEquatable<ThrottlePeriodicEventStreamResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="Object">A get periodic event streams response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ThrottlePeriodicEventStreamResponse throttlePeriodicEventStreamResponse &&
                   Equals(throttlePeriodicEventStreamResponse);

        #endregion

        #region Equals(ThrottlePeriodicEventStreamResponse)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="ThrottlePeriodicEventStreamResponse">A get periodic event streams response to compare with.</param>
        public override Boolean Equals(ThrottlePeriodicEventStreamResponse? ThrottlePeriodicEventStreamResponse)

            => ThrottlePeriodicEventStreamResponse is not null &&

               Status.Equals(ThrottlePeriodicEventStreamResponse.Status) &&

             ((StatusInfo is     null && ThrottlePeriodicEventStreamResponse.StatusInfo is     null) ||
              (StatusInfo is not null && ThrottlePeriodicEventStreamResponse.StatusInfo is not null && StatusInfo.Equals(ThrottlePeriodicEventStreamResponse.StatusInfo))) &&

               base.GenericEquals(ThrottlePeriodicEventStreamResponse);

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

                   Status.AsText(),

                   StatusInfo is not null
                       ? $", {StatusInfo}"
                       : ""

               );

        #endregion


    }

}
