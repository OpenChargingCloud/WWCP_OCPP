/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The NotifyNetworkTimeResult response.
    /// </summary>
    public class NotifyNetworkTimeResultResponse : AResponse<NotifyNetworkTimeResultRequest,
                                               NotifyNetworkTimeResultResponse>,
                                     IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyNetworkTimeResultResponse");

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
        /// Create a new NotifyNetworkTimeResult response.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTimeResult request leading to this response.</param>
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
        public NotifyNetworkTimeResultResponse(CS.NotifyNetworkTimeResultRequest    Request,

                                 Result?                  Result                = null,
                                 DateTimeOffset?          ResponseTimestamp     = null,

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

        #region (static) Parse   (Request, JSON, CustomNotifyNetworkTimeResultResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a NotifyNetworkTimeResult response.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTimeResult request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyNetworkTimeResultResponseParser">A delegate to parse custom NotifyNetworkTimeResult responses.</param>
        public static NotifyNetworkTimeResultResponse Parse(CS.NotifyNetworkTimeResultRequest                            Request,
                                              JObject                                          JSON,
                                              SourceRouting                                Destination,
                                              NetworkPath                                      NetworkPath,
                                              DateTimeOffset?                                  ResponseTimestamp               = null,
                                              CustomJObjectParserDelegate<NotifyNetworkTimeResultResponse>?  CustomNotifyNetworkTimeResultResponseParser   = null,
                                              CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                              CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var notifyNetworkTimeResultResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyNetworkTimeResultResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifyNetworkTimeResultResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyNetworkTimeResult response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyNetworkTimeResultResponse, out ErrorResponse, CustomNotifyNetworkTimeResultResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyNetworkTimeResult response.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTimeResult request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyNetworkTimeResultResponse">The parsed NotifyNetworkTimeResult response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyNetworkTimeResultResponseParser">A delegate to parse custom NotifyNetworkTimeResult responses.</param>
        public static Boolean TryParse(CS.NotifyNetworkTimeResultRequest                            Request,
                                       JObject                                          JSON,
                                       SourceRouting                                Destination,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out NotifyNetworkTimeResultResponse?      NotifyNetworkTimeResultResponse,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       DateTimeOffset?                                  ResponseTimestamp               = null,
                                       CustomJObjectParserDelegate<NotifyNetworkTimeResultResponse>?  CustomNotifyNetworkTimeResultResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                       CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
        {

            try
            {

                NotifyNetworkTimeResultResponse = null;

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


                NotifyNetworkTimeResultResponse = new NotifyNetworkTimeResultResponse(

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

                if (CustomNotifyNetworkTimeResultResponseParser is not null)
                    NotifyNetworkTimeResultResponse = CustomNotifyNetworkTimeResultResponseParser(JSON,
                                                                      NotifyNetworkTimeResultResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyNetworkTimeResultResponse  = null;
                ErrorResponse      = "The given JSON representation of a NotifyNetworkTimeResult response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyNetworkTimeResultResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyNetworkTimeResultResponseSerializer">A delegate to serialize custom NotifyNetworkTimeResult responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                              IncludeJSONLDContext                = false,
                              CustomJObjectSerializerDelegate<NotifyNetworkTimeResultResponse>?  CustomNotifyNetworkTimeResultResponseSerializer   = null,
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

            return CustomNotifyNetworkTimeResultResponseSerializer is not null
                       ? CustomNotifyNetworkTimeResultResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(Request, CBOR, ..., out NotifyNetworkTimeResultResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a NotifyNetworkTimeResult response - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="NotifyNetworkTimeResultResponse">The NotifyNetworkTimeResult response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomNotifyNetworkTimeResultResponseParser">A delegate to read custom NotifyNetworkTimeResult responses.</param>
        public static Boolean TryParseCBOR(CS.NotifyNetworkTimeResultRequest                         Request,
                                           CBORValue                                     CBOR,
                                           SourceRouting                                 Destination,
                                           NetworkPath                                   NetworkPath,
                                           [NotNullWhen(true)]  out NotifyNetworkTimeResultResponse?   NotifyNetworkTimeResultResponse,
                                           [NotNullWhen(false)] out String?              ErrorResponse,
                                           DateTimeOffset?                               ResponseTimestamp               = null,
                                           CustomCBORParserDelegate<NotifyNetworkTimeResultResponse>?  CustomNotifyNetworkTimeResultResponseParser   = null)
        {

            try
            {

                NotifyNetworkTimeResultResponse = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a NotifyNetworkTimeResult response is not a map!";
                    return false;
                }

                CBOR.ParseOptionalList<Signature>("signatures",
                                               "cryptographic signatures",
                                               OCPPCBORExtensions.TryParseSignature,
                                               out var Signatures,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                NotifyNetworkTimeResultResponse = new NotifyNetworkTimeResultResponse(

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

                if (CustomNotifyNetworkTimeResultResponseParser is not null)
                    NotifyNetworkTimeResultResponse = CustomNotifyNetworkTimeResultResponseParser(CBOR,
                                                                     NotifyNetworkTimeResultResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                NotifyNetworkTimeResultResponse = null;
                ErrorResponse = "The given CBOR representation of a NotifyNetworkTimeResult response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomNotifyNetworkTimeResultResponseSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this NotifyNetworkTimeResult response: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomNotifyNetworkTimeResultResponseSerializer">A delegate to serialize custom NotifyNetworkTimeResult responses.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<NotifyNetworkTimeResultResponse>? CustomNotifyNetworkTimeResultResponseSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomNotifyNetworkTimeResultResponseSerializer is not null
                       ? CustomNotifyNetworkTimeResultResponseSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyNetworkTimeResult failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTimeResult request.</param>
        public static NotifyNetworkTimeResultResponse RequestError(CS.NotifyNetworkTimeResultRequest    Request,
                                                     EventTracking_Id         EventTrackingId,
                                                     ResultCode               ErrorCode,
                                                     String?                  ErrorDescription    = null,
                                                     JObject?                 ErrorDetails        = null,
                                                     DateTimeOffset?          ResponseTimestamp   = null,

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
        /// The NotifyNetworkTimeResult failed.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTimeResult request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyNetworkTimeResultResponse FormationViolation(CS.NotifyNetworkTimeResultRequest  Request,
                                                           String                 ErrorDescription)

            => new (Request,
                   OCPPv2_1.Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyNetworkTimeResult failed.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTimeResult request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyNetworkTimeResultResponse SignatureError(CS.NotifyNetworkTimeResultRequest  Request,
                                                       String                 ErrorDescription)

            => new (Request,
                   OCPPv2_1.Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyNetworkTimeResult failed.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTimeResult request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyNetworkTimeResultResponse Failed(CS.NotifyNetworkTimeResultRequest  Request,
                                               String?                Description   = null)

            => new (Request,
                    OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The NotifyNetworkTimeResult failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTimeResult request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyNetworkTimeResultResponse ExceptionOccurred(CS.NotifyNetworkTimeResultRequest  Request,
                                                         Exception              Exception)

            => new (Request,
                    OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyNetworkTimeResultResponse1, NotifyNetworkTimeResultResponse2)

        /// <summary>
        /// Compares two NotifyNetworkTimeResult responses for equality.
        /// </summary>
        /// <param name="NotifyNetworkTimeResultResponse1">A NotifyNetworkTimeResult response.</param>
        /// <param name="NotifyNetworkTimeResultResponse2">Another NotifyNetworkTimeResult response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyNetworkTimeResultResponse? NotifyNetworkTimeResultResponse1,
                                           NotifyNetworkTimeResultResponse? NotifyNetworkTimeResultResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyNetworkTimeResultResponse1, NotifyNetworkTimeResultResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyNetworkTimeResultResponse1 is null || NotifyNetworkTimeResultResponse2 is null)
                return false;

            return NotifyNetworkTimeResultResponse1.Equals(NotifyNetworkTimeResultResponse2);

        }

        #endregion

        #region Operator != (NotifyNetworkTimeResultResponse1, NotifyNetworkTimeResultResponse2)

        /// <summary>
        /// Compares two NotifyNetworkTimeResult responses for inequality.
        /// </summary>
        /// <param name="NotifyNetworkTimeResultResponse1">A NotifyNetworkTimeResult response.</param>
        /// <param name="NotifyNetworkTimeResultResponse2">Another NotifyNetworkTimeResult response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyNetworkTimeResultResponse? NotifyNetworkTimeResultResponse1,
                                           NotifyNetworkTimeResultResponse? NotifyNetworkTimeResultResponse2)

            => !(NotifyNetworkTimeResultResponse1 == NotifyNetworkTimeResultResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyNetworkTimeResultResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyNetworkTimeResult responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyNetworkTimeResult response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyNetworkTimeResultResponse notifyNetworkTimeResultResponse &&
                   Equals(notifyNetworkTimeResultResponse);

        #endregion

        #region Equals(NotifyNetworkTimeResultResponse)

        /// <summary>
        /// Compares two NotifyNetworkTimeResult responses for equality.
        /// </summary>
        /// <param name="NotifyNetworkTimeResultResponse">A NotifyNetworkTimeResult response to compare with.</param>
        public override Boolean Equals(NotifyNetworkTimeResultResponse? NotifyNetworkTimeResultResponse)

            => NotifyNetworkTimeResultResponse is not null &&

               base.GenericEquals(NotifyNetworkTimeResultResponse);

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

            => CustomData?.ToString() ?? "NotifyNetworkTimeResultResponse";

        #endregion


    }

}
