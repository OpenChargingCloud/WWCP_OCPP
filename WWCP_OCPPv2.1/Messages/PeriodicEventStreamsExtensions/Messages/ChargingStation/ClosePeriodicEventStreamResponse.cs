/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/CloseChargingCloud/WWCP_OCPP>
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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// An close periodic event stream response.
    /// </summary>
    public class ClosePeriodicEventStreamResponse : AResponse<CS.ClosePeriodicEventStreamRequest,
                                                                 ClosePeriodicEventStreamResponse>,
                                                    IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/closePeriodicEventStreamResponse");

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

        #region ClosePeriodicEventStreamResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new close periodic event stream response.
        /// </summary>
        /// <param name="Request">The close periodic event stream request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ClosePeriodicEventStreamResponse(CS.ClosePeriodicEventStreamRequest  Request,
                                                GenericStatus                       Status,
                                                StatusInfo?                         StatusInfo          = null,
                                                DateTime?                           ResponseTimestamp   = null,

                                                IEnumerable<KeyPair>?               SignKeys            = null,
                                                IEnumerable<SignInfo>?              SignInfos           = null,
                                                IEnumerable<Signature>?             Signatures          = null,

                                                CustomData?                         CustomData          = null)

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

        #region ClosePeriodicEventStreamResponse(Request, Result)

        /// <summary>
        /// Create a new close periodic event stream response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public ClosePeriodicEventStreamResponse(CS.ClosePeriodicEventStreamRequest  Request,
                                                Result                              Result)

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

        #region (static) Parse   (Request, JSON, CustomClosePeriodicEventStreamResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an close periodic event stream response.
        /// </summary>
        /// <param name="Request">The close periodic event stream request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClosePeriodicEventStreamResponseParser">A delegate to parse custom close periodic event stream responses.</param>
        public static ClosePeriodicEventStreamResponse Parse(CS.ClosePeriodicEventStreamRequest                              Request,
                                                             JObject                                                         JSON,
                                                             CustomJObjectParserDelegate<ClosePeriodicEventStreamResponse>?  CustomClosePeriodicEventStreamResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var closePeriodicEventStreamResponse,
                         out var errorResponse,
                         CustomClosePeriodicEventStreamResponseParser) &&
                closePeriodicEventStreamResponse is not null)
            {
                return closePeriodicEventStreamResponse;
            }

            throw new ArgumentException("The given JSON representation of an close periodic event stream response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ClosePeriodicEventStreamResponse, out ErrorResponse, CustomClosePeriodicEventStreamResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an close periodic event stream response.
        /// </summary>
        /// <param name="Request">The close periodic event stream request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClosePeriodicEventStreamResponse">The parsed close periodic event stream response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClosePeriodicEventStreamResponseParser">A delegate to parse custom close periodic event stream responses.</param>
        public static Boolean TryParse(CS.ClosePeriodicEventStreamRequest                              Request,
                                       JObject                                                         JSON,
                                       out ClosePeriodicEventStreamResponse?                           ClosePeriodicEventStreamResponse,
                                       out String?                                                     ErrorResponse,
                                       CustomJObjectParserDelegate<ClosePeriodicEventStreamResponse>?  CustomClosePeriodicEventStreamResponseParser   = null)
        {

            try
            {

                ClosePeriodicEventStreamResponse = null;

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
                                           OCPPv2_1.StatusInfo.TryParse,
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


                ClosePeriodicEventStreamResponse = new ClosePeriodicEventStreamResponse(
                                                       Request,
                                                       RegistrationStatus,
                                                       StatusInfo,
                                                       null,
                                                       null,
                                                       null,
                                                       Signatures,
                                                       CustomData
                                                   );

                if (CustomClosePeriodicEventStreamResponseParser is not null)
                    ClosePeriodicEventStreamResponse = CustomClosePeriodicEventStreamResponseParser(JSON,
                                                                                                    ClosePeriodicEventStreamResponse);

                return true;

            }
            catch (Exception e)
            {
                ClosePeriodicEventStreamResponse  = null;
                ErrorResponse                     = "The given JSON representation of an close periodic event stream response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClosePeriodicEventStreamResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClosePeriodicEventStreamResponseSerializer">A delegate to serialize custom close periodic event stream responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClosePeriodicEventStreamResponse>?  CustomClosePeriodicEventStreamResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                        CustomStatusInfoSerializer                         = null,
                              CustomJObjectSerializerDelegate<Signature>?                         CustomSignatureSerializer                          = null,
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

            return CustomClosePeriodicEventStreamResponseSerializer is not null
                       ? CustomClosePeriodicEventStreamResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The close periodic event stream failed.
        /// </summary>
        public static ClosePeriodicEventStreamResponse Failed(CS.ClosePeriodicEventStreamRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ClosePeriodicEventStreamResponse1, ClosePeriodicEventStreamResponse2)

        /// <summary>
        /// Compares two close periodic event stream responses for equality.
        /// </summary>
        /// <param name="ClosePeriodicEventStreamResponse1">An close periodic event stream response.</param>
        /// <param name="ClosePeriodicEventStreamResponse2">Another close periodic event stream response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClosePeriodicEventStreamResponse? ClosePeriodicEventStreamResponse1,
                                           ClosePeriodicEventStreamResponse? ClosePeriodicEventStreamResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClosePeriodicEventStreamResponse1, ClosePeriodicEventStreamResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ClosePeriodicEventStreamResponse1 is null || ClosePeriodicEventStreamResponse2 is null)
                return false;

            return ClosePeriodicEventStreamResponse1.Equals(ClosePeriodicEventStreamResponse2);

        }

        #endregion

        #region Operator != (ClosePeriodicEventStreamResponse1, ClosePeriodicEventStreamResponse2)

        /// <summary>
        /// Compares two close periodic event stream responses for inequality.
        /// </summary>
        /// <param name="ClosePeriodicEventStreamResponse1">An close periodic event stream response.</param>
        /// <param name="ClosePeriodicEventStreamResponse2">Another close periodic event stream response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClosePeriodicEventStreamResponse? ClosePeriodicEventStreamResponse1,
                                           ClosePeriodicEventStreamResponse? ClosePeriodicEventStreamResponse2)

            => !(ClosePeriodicEventStreamResponse1 == ClosePeriodicEventStreamResponse2);

        #endregion

        #endregion

        #region IEquatable<ClosePeriodicEventStreamResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two close periodic event stream responses for equality.
        /// </summary>
        /// <param name="Object">An close periodic event stream response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClosePeriodicEventStreamResponse closePeriodicEventStreamResponse &&
                   Equals(closePeriodicEventStreamResponse);

        #endregion

        #region Equals(ClosePeriodicEventStreamResponse)

        /// <summary>
        /// Compares two close periodic event stream responses for equality.
        /// </summary>
        /// <param name="ClosePeriodicEventStreamResponse">An close periodic event stream response to compare with.</param>
        public override Boolean Equals(ClosePeriodicEventStreamResponse? ClosePeriodicEventStreamResponse)

            => ClosePeriodicEventStreamResponse is not null &&

               Status.Equals(ClosePeriodicEventStreamResponse.Status) &&

             ((StatusInfo is     null && ClosePeriodicEventStreamResponse.StatusInfo is     null) ||
              (StatusInfo is not null && ClosePeriodicEventStreamResponse.StatusInfo is not null && StatusInfo.Equals(ClosePeriodicEventStreamResponse.StatusInfo))) &&

               base.GenericEquals(ClosePeriodicEventStreamResponse);

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
