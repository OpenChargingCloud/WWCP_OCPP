/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A notify notify certificate revocation list response.
    /// </summary>
    public class NotifyCRLResponse : AResponse<CSMS.NotifyCRLRequest,
                                                    NotifyCRLResponse>,
                                     IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/notifyCRLResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        #region NotifyCRLResponse(Request, ...)

        /// <summary>
        /// Create a new notify certificate revocation list response.
        /// </summary>
        /// <param name="Request">The notify certificate revocation list request leading to this response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public NotifyCRLResponse(CSMS.NotifyCRLRequest    Request,
                                 DateTime?                ResponseTimestamp   = null,

                                 IEnumerable<KeyPair>?    SignKeys            = null,
                                 IEnumerable<SignInfo>?   SignInfos           = null,
                                 IEnumerable<OCPP.Signature>?  Signatures          = null,

                                 CustomData?              CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #region NotifyCRLResponse(Request, Result)

        /// <summary>
        /// Create a new notify certificate revocation list response.
        /// </summary>
        /// <param name="Request">The notify certificate revocation list request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyCRLResponse(CSMS.NotifyCRLRequest  Request,
                                 Result                 Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomNotifyCRLResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify certificate revocation list response.
        /// </summary>
        /// <param name="Request">The notify certificate revocation list request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyCRLResponseParser">A delegate to parse custom notify certificate revocation list responses.</param>
        public static NotifyCRLResponse Parse(CSMS.NotifyCRLRequest                            Request,
                                              JObject                                          JSON,
                                              CustomJObjectParserDelegate<NotifyCRLResponse>?  CustomNotifyCRLResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var notifyCRLResponse,
                         out var errorResponse,
                         CustomNotifyCRLResponseParser) &&
                notifyCRLResponse is not null)
            {
                return notifyCRLResponse;
            }

            throw new ArgumentException("The given JSON representation of a notify certificate revocation list response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyCRLResponse, out ErrorResponse, CustomNotifyCRLResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify certificate revocation list response.
        /// </summary>
        /// <param name="Request">The notify certificate revocation list request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyCRLResponse">The parsed notify certificate revocation list response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyCRLResponseParser">A delegate to parse custom notify certificate revocation list responses.</param>
        public static Boolean TryParse(CSMS.NotifyCRLRequest                            Request,
                                       JObject                                          JSON,
                                       out NotifyCRLResponse?                           NotifyCRLResponse,
                                       out String?                                      ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyCRLResponse>?  CustomNotifyCRLResponseParser   = null)
        {

            try
            {

                NotifyCRLResponse = null;

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


                NotifyCRLResponse = new NotifyCRLResponse(
                                        Request,
                                        null,
                                        null,
                                        null,
                                        Signatures,
                                        CustomData
                                    );

                if (CustomNotifyCRLResponseParser is not null)
                    NotifyCRLResponse = CustomNotifyCRLResponseParser(JSON,
                                                                      NotifyCRLResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyCRLResponse  = null;
                ErrorResponse      = "The given JSON representation of a notify certificate revocation list response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyCRLResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyCRLResponseSerializer">A delegate to serialize custom notify certificate revocation list responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyCRLResponse>?  CustomNotifyCRLResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?     CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyCRLResponseSerializer is not null
                       ? CustomNotifyCRLResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The notify certificate revocation list request failed.
        /// </summary>
        /// <param name="Request">The notify certificate revocation list request leading to this response.</param>
        public static NotifyCRLResponse Failed(CSMS.NotifyCRLRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (NotifyCRLResponse1, NotifyCRLResponse2)

        /// <summary>
        /// Compares two notify certificate revocation list responses for equality.
        /// </summary>
        /// <param name="NotifyCRLResponse1">A notify certificate revocation list response.</param>
        /// <param name="NotifyCRLResponse2">Another notify certificate revocation list response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyCRLResponse? NotifyCRLResponse1,
                                           NotifyCRLResponse? NotifyCRLResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyCRLResponse1, NotifyCRLResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyCRLResponse1 is null || NotifyCRLResponse2 is null)
                return false;

            return NotifyCRLResponse1.Equals(NotifyCRLResponse2);

        }

        #endregion

        #region Operator != (NotifyCRLResponse1, NotifyCRLResponse2)

        /// <summary>
        /// Compares two notify certificate revocation list responses for inequality.
        /// </summary>
        /// <param name="NotifyCRLResponse1">A notify certificate revocation list response.</param>
        /// <param name="NotifyCRLResponse2">Another notify certificate revocation list response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyCRLResponse? NotifyCRLResponse1,
                                           NotifyCRLResponse? NotifyCRLResponse2)

            => !(NotifyCRLResponse1 == NotifyCRLResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyCRLResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify certificate revocation list responses for equality.
        /// </summary>
        /// <param name="Object">A notify certificate revocation list response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyCRLResponse notifyCRLResponse &&
                   Equals(notifyCRLResponse);

        #endregion

        #region Equals(NotifyCRLResponse)

        /// <summary>
        /// Compares two notify certificate revocation list responses for equality.
        /// </summary>
        /// <param name="NotifyCRLResponse">A notify certificate revocation list response to compare with.</param>
        public override Boolean Equals(NotifyCRLResponse? NotifyCRLResponse)

            => NotifyCRLResponse is not null &&

               base.GenericEquals(NotifyCRLResponse);

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

                return base.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => CustomData?.ToString() ?? "NotifyCRLResponse";

        #endregion


    }

}
