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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The NotifyCRL response.
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
        /// Create a new NotifyCRL response.
        /// </summary>
        /// <param name="Request">The NotifyCRL request leading to this response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public NotifyCRLResponse(CSMS.NotifyCRLRequest         Request,
                                 DateTime?                     ResponseTimestamp   = null,

                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                 IEnumerable<Signature>?       Signatures          = null,

                                 CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #region NotifyCRLResponse(Request, Result)

        /// <summary>
        /// Create a new NotifyCRL response.
        /// </summary>
        /// <param name="Request">The NotifyCRL request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyCRLResponse(CSMS.NotifyCRLRequest    Request,
                                 Result                   Result,
                                 DateTime?                ResponseTimestamp   = null,

                                 NetworkingNode_Id?       DestinationId       = null,
                                 NetworkPath?             NetworkPath         = null,

                                 IEnumerable<KeyPair>?    SignKeys            = null,
                                 IEnumerable<SignInfo>?   SignInfos           = null,
                                 IEnumerable<Signature>?  Signatures          = null,

                                 CustomData?              CustomData          = null)

            : base(Request,
                   Result,
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomNotifyCRLResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a NotifyCRL response.
        /// </summary>
        /// <param name="Request">The NotifyCRL request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyCRLResponseParser">A delegate to parse custom NotifyCRL responses.</param>
        public static NotifyCRLResponse Parse(CSMS.NotifyCRLRequest                            Request,
                                              JObject                                          JSON,
                                              CustomJObjectParserDelegate<NotifyCRLResponse>?  CustomNotifyCRLResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var notifyCRLResponse,
                         out var errorResponse,
                         CustomNotifyCRLResponseParser))
            {
                return notifyCRLResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyCRL response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyCRLResponse, out ErrorResponse, CustomNotifyCRLResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyCRL response.
        /// </summary>
        /// <param name="Request">The NotifyCRL request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyCRLResponse">The parsed NotifyCRL response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyCRLResponseParser">A delegate to parse custom NotifyCRL responses.</param>
        public static Boolean TryParse(CSMS.NotifyCRLRequest                            Request,
                                       JObject                                          JSON,
                                       [NotNullWhen(true)]  out NotifyCRLResponse?      NotifyCRLResponse,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyCRLResponse>?  CustomNotifyCRLResponseParser   = null)
        {

            try
            {

                NotifyCRLResponse = null;

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
                ErrorResponse      = "The given JSON representation of a NotifyCRL response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyCRLResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyCRLResponseSerializer">A delegate to serialize custom NotifyCRL responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyCRLResponse>?  CustomNotifyCRLResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
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
        /// The NotifyCRL failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyCRL request.</param>
        public static NotifyCRLResponse RequestError(CSMS.NotifyCRLRequest    Request,
                                                     EventTracking_Id         EventTrackingId,
                                                     ResultCode               ErrorCode,
                                                     String?                  ErrorDescription    = null,
                                                     JObject?                 ErrorDetails        = null,
                                                     DateTime?                ResponseTimestamp   = null,

                                                     NetworkingNode_Id?       DestinationId       = null,
                                                     NetworkPath?             NetworkPath         = null,

                                                     IEnumerable<KeyPair>?    SignKeys            = null,
                                                     IEnumerable<SignInfo>?   SignInfos           = null,
                                                     IEnumerable<Signature>?  Signatures          = null,

                                                     CustomData?              CustomData          = null)

            => new (

                   Request,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The NotifyCRL failed.
        /// </summary>
        /// <param name="Request">The NotifyCRL request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyCRLResponse SignatureError(CSMS.NotifyCRLRequest  Request,
                                                       String                 ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyCRL failed.
        /// </summary>
        /// <param name="Request">The NotifyCRL request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyCRLResponse Failed(CSMS.NotifyCRLRequest  Request,
                                               String?                Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The NotifyCRL failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyCRL request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyCRLResponse ExceptionOccured(CSMS.NotifyCRLRequest  Request,
                                                         Exception              Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyCRLResponse1, NotifyCRLResponse2)

        /// <summary>
        /// Compares two NotifyCRL responses for equality.
        /// </summary>
        /// <param name="NotifyCRLResponse1">A NotifyCRL response.</param>
        /// <param name="NotifyCRLResponse2">Another NotifyCRL response.</param>
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
        /// Compares two NotifyCRL responses for inequality.
        /// </summary>
        /// <param name="NotifyCRLResponse1">A NotifyCRL response.</param>
        /// <param name="NotifyCRLResponse2">Another NotifyCRL response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyCRLResponse? NotifyCRLResponse1,
                                           NotifyCRLResponse? NotifyCRLResponse2)

            => !(NotifyCRLResponse1 == NotifyCRLResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyCRLResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyCRL responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyCRL response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyCRLResponse notifyCRLResponse &&
                   Equals(notifyCRLResponse);

        #endregion

        #region Equals(NotifyCRLResponse)

        /// <summary>
        /// Compares two NotifyCRL responses for equality.
        /// </summary>
        /// <param name="NotifyCRLResponse">A NotifyCRL response to compare with.</param>
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
