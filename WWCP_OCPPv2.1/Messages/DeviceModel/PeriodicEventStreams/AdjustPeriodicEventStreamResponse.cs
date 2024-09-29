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

using cloud.charging.open.protocols.WWCP;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A AdjustPeriodicEventStream response.
    /// </summary>
    public class AdjustPeriodicEventStreamResponse : AResponse<AdjustPeriodicEventStreamRequest,
                                                               AdjustPeriodicEventStreamResponse>,
                                                     IResponse<Result>
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

        #region AdjustPeriodicEventStreamResponse(Request, Status, StatusInfo = null, ...)

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
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public AdjustPeriodicEventStreamResponse(CSMS.AdjustPeriodicEventStreamRequest  Request,
                                                   GenericStatus                            Status,
                                                   StatusInfo?                              StatusInfo          = null,
                                                   DateTime?                                ResponseTimestamp   = null,

                                                   IEnumerable<KeyPair>?                    SignKeys            = null,
                                                   IEnumerable<SignInfo>?                   SignInfos           = null,
                                                   IEnumerable<Signature>?                  Signatures          = null,

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

        #region AdjustPeriodicEventStreamResponse(Request, Result)

        /// <summary>
        /// Create a new open periodic event stream response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public AdjustPeriodicEventStreamResponse(CSMS.AdjustPeriodicEventStreamRequest  Request,
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

        #region (static) Parse   (Request, JSON, CustomAdjustPeriodicEventStreamResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get periodic event streams response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAdjustPeriodicEventStreamResponseParser">A delegate to parse custom open periodic event stream responses.</param>
        public static AdjustPeriodicEventStreamResponse Parse(CSMS.AdjustPeriodicEventStreamRequest                            Request,
                                                                JObject                                                            JSON,
                                                                CustomJObjectParserDelegate<AdjustPeriodicEventStreamResponse>?  CustomAdjustPeriodicEventStreamResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var throttlePeriodicEventStreamResponse,
                         out var errorResponse,
                         CustomAdjustPeriodicEventStreamResponseParser) &&
                throttlePeriodicEventStreamResponse is not null)
            {
                return throttlePeriodicEventStreamResponse;
            }

            throw new ArgumentException("The given JSON representation of a get periodic event streams response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out AdjustPeriodicEventStreamResponse, out ErrorResponse, CustomAdjustPeriodicEventStreamResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get periodic event streams response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AdjustPeriodicEventStreamResponse">The parsed open periodic event stream response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAdjustPeriodicEventStreamResponseParser">A delegate to parse custom open periodic event stream responses.</param>
        public static Boolean TryParse(CSMS.AdjustPeriodicEventStreamRequest                            Request,
                                       JObject                                                            JSON,
                                       [NotNullWhen(true)]  out AdjustPeriodicEventStreamResponse?      AdjustPeriodicEventStreamResponse,
                                       [NotNullWhen(false)] out String?                                   ErrorResponse,
                                       CustomJObjectParserDelegate<AdjustPeriodicEventStreamResponse>?  CustomAdjustPeriodicEventStreamResponseParser   = null)
        {

            try
            {

                AdjustPeriodicEventStreamResponse = null;

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
                                           OCPPv2_1.StatusInfo.TryParse,
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                AdjustPeriodicEventStreamResponse = new AdjustPeriodicEventStreamResponse(

                                                          Request,
                                                          Status,
                                                          StatusInfo,
                                                          null,

                                                          null,
                                                          null,
                                                          Signatures,

                                                          CustomData

                                                      );

                if (CustomAdjustPeriodicEventStreamResponseParser is not null)
                    AdjustPeriodicEventStreamResponse = CustomAdjustPeriodicEventStreamResponseParser(JSON,
                                                                                                          AdjustPeriodicEventStreamResponse);

                return true;

            }
            catch (Exception e)
            {
                AdjustPeriodicEventStreamResponse  = null;
                ErrorResponse                        = "The given JSON representation of a AdjustPeriodicEventStreamResponse is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAdjustPeriodicEventStreamResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAdjustPeriodicEventStreamResponseSerializer">A delegate to serialize custom open periodic event stream responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AdjustPeriodicEventStreamResponse>?  CustomAdjustPeriodicEventStreamResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                           CustomStatusInfoSerializer                            = null,
                              CustomJObjectSerializerDelegate<Signature>?                            CustomSignatureSerializer                             = null,
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

            return CustomAdjustPeriodicEventStreamResponseSerializer is not null
                       ? CustomAdjustPeriodicEventStreamResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The open periodic event stream failed.
        /// </summary>
        public static AdjustPeriodicEventStreamResponse Failed(CSMS.AdjustPeriodicEventStreamRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (AdjustPeriodicEventStreamResponse1, AdjustPeriodicEventStreamResponse2)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="AdjustPeriodicEventStreamResponse1">A get periodic event streams response.</param>
        /// <param name="AdjustPeriodicEventStreamResponse2">Another open periodic event stream response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AdjustPeriodicEventStreamResponse? AdjustPeriodicEventStreamResponse1,
                                           AdjustPeriodicEventStreamResponse? AdjustPeriodicEventStreamResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AdjustPeriodicEventStreamResponse1, AdjustPeriodicEventStreamResponse2))
                return true;

            // If one is null, but not both, return false.
            if (AdjustPeriodicEventStreamResponse1 is null || AdjustPeriodicEventStreamResponse2 is null)
                return false;

            return AdjustPeriodicEventStreamResponse1.Equals(AdjustPeriodicEventStreamResponse2);

        }

        #endregion

        #region Operator != (AdjustPeriodicEventStreamResponse1, AdjustPeriodicEventStreamResponse2)

        /// <summary>
        /// Compares two open periodic event stream responses for inequality.
        /// </summary>
        /// <param name="AdjustPeriodicEventStreamResponse1">A get periodic event streams response.</param>
        /// <param name="AdjustPeriodicEventStreamResponse2">Another open periodic event stream response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AdjustPeriodicEventStreamResponse? AdjustPeriodicEventStreamResponse1,
                                           AdjustPeriodicEventStreamResponse? AdjustPeriodicEventStreamResponse2)

            => !(AdjustPeriodicEventStreamResponse1 == AdjustPeriodicEventStreamResponse2);

        #endregion

        #endregion

        #region IEquatable<AdjustPeriodicEventStreamResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="Object">A get periodic event streams response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AdjustPeriodicEventStreamResponse throttlePeriodicEventStreamResponse &&
                   Equals(throttlePeriodicEventStreamResponse);

        #endregion

        #region Equals(AdjustPeriodicEventStreamResponse)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="AdjustPeriodicEventStreamResponse">A get periodic event streams response to compare with.</param>
        public override Boolean Equals(AdjustPeriodicEventStreamResponse? AdjustPeriodicEventStreamResponse)

            => AdjustPeriodicEventStreamResponse is not null &&

               Status.Equals(AdjustPeriodicEventStreamResponse.Status) &&

             ((StatusInfo is     null && AdjustPeriodicEventStreamResponse.StatusInfo is     null) ||
              (StatusInfo is not null && AdjustPeriodicEventStreamResponse.StatusInfo is not null && StatusInfo.Equals(AdjustPeriodicEventStreamResponse.StatusInfo))) &&

               base.GenericEquals(AdjustPeriodicEventStreamResponse);

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
