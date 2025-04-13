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
    /// The NotifyCRL response.
    /// </summary>
    public class NotifyCRLResponse : AResponse<NotifyCRLRequest,
                                               NotifyCRLResponse>,
                                     IResponse<Result>
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

        /// <summary>
        /// Create a new NotifyCRL response.
        /// </summary>
        /// <param name="Request">The NotifyCRL request leading to this response.</param>
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
        public NotifyCRLResponse(CSMS.NotifyCRLRequest    Request,

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

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        { }

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
                                              SourceRouting                                Destination,
                                              NetworkPath                                      NetworkPath,
                                              DateTime?                                        ResponseTimestamp               = null,
                                              CustomJObjectParserDelegate<NotifyCRLResponse>?  CustomNotifyCRLResponseParser   = null,
                                              CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                              CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var notifyCRLResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyCRLResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
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
                                       SourceRouting                                Destination,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out NotifyCRLResponse?      NotifyCRLResponse,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       DateTime?                                        ResponseTimestamp               = null,
                                       CustomJObjectParserDelegate<NotifyCRLResponse>?  CustomNotifyCRLResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                       CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
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
                                           WWCP.CustomData.TryParse,
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
                                        ResponseTimestamp,

                                        Destination,
                                        NetworkPath,

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
        public JObject ToJSON(Boolean                                              IncludeJSONLDContext                = false,
                              CustomJObjectSerializerDelegate<NotifyCRLResponse>?  CustomNotifyCRLResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
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

                                                     SourceRouting?           Destination         = null,
                                                     NetworkPath?             NetworkPath         = null,

                                                     IEnumerable<KeyPair>?    SignKeys            = null,
                                                     IEnumerable<SignInfo>?   SignInfos           = null,
                                                     IEnumerable<Signature>?  Signatures          = null,

                                                     CustomData?              CustomData          = null)

            => new (

                   Request,
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
        /// The NotifyCRL failed.
        /// </summary>
        /// <param name="Request">The NotifyCRL request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyCRLResponse FormationViolation(CSMS.NotifyCRLRequest  Request,
                                                           String                 ErrorDescription)

            => new (Request,
                   OCPPv2_1.Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyCRL failed.
        /// </summary>
        /// <param name="Request">The NotifyCRL request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyCRLResponse SignatureError(CSMS.NotifyCRLRequest  Request,
                                                       String                 ErrorDescription)

            => new (Request,
                   OCPPv2_1.Result.SignatureError(
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
                    OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The NotifyCRL failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyCRL request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyCRLResponse ExceptionOccurred(CSMS.NotifyCRLRequest  Request,
                                                         Exception              Exception)

            => new (Request,
                    OCPPv2_1.Result.FromException(Exception));

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
