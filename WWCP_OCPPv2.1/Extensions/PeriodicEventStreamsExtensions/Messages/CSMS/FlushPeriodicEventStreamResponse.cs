/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/FlushChargingCloud/WWCP_OCPP>
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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A flush periodic event stream response.
    /// </summary>
    public class FlushPeriodicEventStreamResponse : AResponse<CSMS.FlushPeriodicEventStreamRequest,
                                                                   FlushPeriodicEventStreamResponse>,
                                                    IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/flushPeriodicEventStreamResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The registration status.
        /// </summary>
        [Mandatory]
        public GenericStatus   Status        { get; }

        /// <summary>
        /// An optional element providing more information about the registration status.
        /// </summary>
        [Optional]
        public StatusInfo?     StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region FlushPeriodicEventStreamResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new open periodic event stream response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public FlushPeriodicEventStreamResponse(CSMS.FlushPeriodicEventStreamRequest  Request,
                                                GenericStatus                         Status,
                                                StatusInfo?                           StatusInfo          = null,
                                                DateTime?                             ResponseTimestamp   = null,

                                                IEnumerable<KeyPair>?                 SignKeys            = null,
                                                IEnumerable<SignInfo>?                SignInfos           = null,
                                                IEnumerable<OCPP.Signature>?          Signatures          = null,

                                                CustomData?                           CustomData          = null)

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

        #region FlushPeriodicEventStreamResponse(Request, Result)

        /// <summary>
        /// Create a new open periodic event stream response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public FlushPeriodicEventStreamResponse(CSMS.FlushPeriodicEventStreamRequest  Request,
                                                Result                                Result)

            : base(Request,
                   Result)

        {

            this.Status = GenericStatus.Rejected;

        }

        #endregion

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomFlushPeriodicEventStreamResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a flush periodic event stream response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomFlushPeriodicEventStreamResponseParser">A delegate to parse custom open periodic event stream responses.</param>
        public static FlushPeriodicEventStreamResponse Parse(CSMS.FlushPeriodicEventStreamRequest                            Request,
                                                             JObject                                                         JSON,
                                                             CustomJObjectParserDelegate<FlushPeriodicEventStreamResponse>?  CustomFlushPeriodicEventStreamResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var flushPeriodicEventStreamResponse,
                         out var errorResponse,
                         CustomFlushPeriodicEventStreamResponseParser) &&
                flushPeriodicEventStreamResponse is not null)
            {
                return flushPeriodicEventStreamResponse;
            }

            throw new ArgumentException("The given JSON representation of a flush periodic event stream response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out FlushPeriodicEventStreamResponse, out ErrorResponse, CustomFlushPeriodicEventStreamResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a flush periodic event stream response.
        /// </summary>
        /// <param name="Request">The open periodic event stream request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="FlushPeriodicEventStreamResponse">The parsed open periodic event stream response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomFlushPeriodicEventStreamResponseParser">A delegate to parse custom open periodic event stream responses.</param>
        public static Boolean TryParse(CSMS.FlushPeriodicEventStreamRequest                            Request,
                                       JObject                                                         JSON,
                                       out FlushPeriodicEventStreamResponse?                           FlushPeriodicEventStreamResponse,
                                       out String?                                                     ErrorResponse,
                                       CustomJObjectParserDelegate<FlushPeriodicEventStreamResponse>?  CustomFlushPeriodicEventStreamResponseParser   = null)
        {

            try
            {

                FlushPeriodicEventStreamResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "registration status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus RegistrationStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (RegistrationStatus == GenericStatus.Unknown)
                {
                    ErrorResponse = "Unknown registration status '" + (JSON["status"]?.Value<String>() ?? "") + "' received!";
                    return false;
                }

                #endregion

                #region StatusInfo     [optional]

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

                #region Signatures     [optional, OCPP_CSE]

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

                #region CustomData     [optional]

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


                FlushPeriodicEventStreamResponse = new FlushPeriodicEventStreamResponse(
                                                       Request,
                                                       RegistrationStatus,
                                                       StatusInfo,
                                                       null,
                                                       null,
                                                       null,
                                                       Signatures,
                                                       CustomData
                                                   );

                if (CustomFlushPeriodicEventStreamResponseParser is not null)
                    FlushPeriodicEventStreamResponse = CustomFlushPeriodicEventStreamResponseParser(JSON,
                                                                                                    FlushPeriodicEventStreamResponse);

                return true;

            }
            catch (Exception e)
            {
                FlushPeriodicEventStreamResponse  = null;
                ErrorResponse                     = "The given JSON representation of a flush periodic event stream response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomFlushPeriodicEventStreamResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFlushPeriodicEventStreamResponseSerializer">A delegate to serialize custom open periodic event stream responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<FlushPeriodicEventStreamResponse>?  CustomFlushPeriodicEventStreamResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                        CustomStatusInfoSerializer                         = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                    CustomSignatureSerializer                          = null,
                              CustomJObjectSerializerDelegate<CustomData>?                        CustomCustomDataSerializer                         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",        Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",    StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                  CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomFlushPeriodicEventStreamResponseSerializer is not null
                       ? CustomFlushPeriodicEventStreamResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The open periodic event stream failed.
        /// </summary>
        public static FlushPeriodicEventStreamResponse Failed(CSMS.FlushPeriodicEventStreamRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (FlushPeriodicEventStreamResponse1, FlushPeriodicEventStreamResponse2)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="FlushPeriodicEventStreamResponse1">A flush periodic event stream response.</param>
        /// <param name="FlushPeriodicEventStreamResponse2">Another open periodic event stream response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (FlushPeriodicEventStreamResponse? FlushPeriodicEventStreamResponse1,
                                           FlushPeriodicEventStreamResponse? FlushPeriodicEventStreamResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(FlushPeriodicEventStreamResponse1, FlushPeriodicEventStreamResponse2))
                return true;

            // If one is null, but not both, return false.
            if (FlushPeriodicEventStreamResponse1 is null || FlushPeriodicEventStreamResponse2 is null)
                return false;

            return FlushPeriodicEventStreamResponse1.Equals(FlushPeriodicEventStreamResponse2);

        }

        #endregion

        #region Operator != (FlushPeriodicEventStreamResponse1, FlushPeriodicEventStreamResponse2)

        /// <summary>
        /// Compares two open periodic event stream responses for inequality.
        /// </summary>
        /// <param name="FlushPeriodicEventStreamResponse1">A flush periodic event stream response.</param>
        /// <param name="FlushPeriodicEventStreamResponse2">Another open periodic event stream response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (FlushPeriodicEventStreamResponse? FlushPeriodicEventStreamResponse1,
                                           FlushPeriodicEventStreamResponse? FlushPeriodicEventStreamResponse2)

            => !(FlushPeriodicEventStreamResponse1 == FlushPeriodicEventStreamResponse2);

        #endregion

        #endregion

        #region IEquatable<FlushPeriodicEventStreamResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="Object">A flush periodic event stream response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is FlushPeriodicEventStreamResponse flushPeriodicEventStreamResponse &&
                   Equals(flushPeriodicEventStreamResponse);

        #endregion

        #region Equals(FlushPeriodicEventStreamResponse)

        /// <summary>
        /// Compares two open periodic event stream responses for equality.
        /// </summary>
        /// <param name="FlushPeriodicEventStreamResponse">A flush periodic event stream response to compare with.</param>
        public override Boolean Equals(FlushPeriodicEventStreamResponse? FlushPeriodicEventStreamResponse)

            => FlushPeriodicEventStreamResponse is not null &&

               Status.Equals(FlushPeriodicEventStreamResponse.Status) &&

             ((StatusInfo is     null && FlushPeriodicEventStreamResponse.StatusInfo is     null) ||
              (StatusInfo is not null && FlushPeriodicEventStreamResponse.StatusInfo is not null && StatusInfo.Equals(FlushPeriodicEventStreamResponse.StatusInfo))) &&

               base.GenericEquals(FlushPeriodicEventStreamResponse);

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

            => Status.AsText();

        #endregion


    }

}
