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
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A GetPWMValue response.
    /// </summary>
    public class GetPWMValueResponse : AResponse<GetPWMValueRequest,
                                                 GetPWMValueResponse>,
                                       IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/getPWMValueResponse");

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
        public GenericStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetPWMValue response.
        /// </summary>
        /// <param name="Request">The GetPWMValue request leading to this response.</param>
        /// <param name="Status">The response status.</param>
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
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetPWMValueResponse(GetPWMValueRequest       Request,
                                   GenericStatus            Status,

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

                   SerializationFormat,
                   CancellationToken)

        {

            this.Status = Status;

            unchecked
            {

                hashCode = this.Status.GetHashCode() * 3 ^
                           base.       GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetPWMValue response.
        /// </summary>
        /// <param name="Request">The GetPWMValue request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomGetPWMValueResponseParser">An optional delegate to parse custom GetPWMValue responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static GetPWMValueResponse Parse(GetPWMValueRequest                                 Request,
                                                JObject                                            JSON,
                                                SourceRouting                                      Destination,
                                                NetworkPath                                        NetworkPath,
                                                DateTimeOffset?                                    ResponseTimestamp                 = null,
                                                CustomJObjectParserDelegate<GetPWMValueResponse>?  CustomGetPWMValueResponseParser   = null,
                                                CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                                CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getPWMValueResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetPWMValueResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getPWMValueResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetPWMValue response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out GetPWMValueResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetPWMValue response.
        /// </summary>
        /// <param name="Request">The GetPWMValue request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="GetPWMValueResponse">The parsed GetPWMValue response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomGetPWMValueResponseParser">An optional delegate to parse custom GetPWMValue responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(GetPWMValueRequest                                 Request,
                                       JObject                                            JSON,
                                       SourceRouting                                      Destination,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out GetPWMValueResponse?      GetPWMValueResponse,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTimeOffset?                                    ResponseTimestamp                 = null,
                                       CustomJObjectParserDelegate<GetPWMValueResponse>?  CustomGetPWMValueResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                       CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            try
            {

                GetPWMValueResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic status",
                                         GenericStatus.TryParse,
                                         out GenericStatus status,
                                         out ErrorResponse))
                {
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


                GetPWMValueResponse = new GetPWMValueResponse(

                                          Request,
                                          status,

                                          null,
                                          ResponseTimestamp,

                                          Destination,
                                          NetworkPath,

                                          null,
                                          null,
                                          Signatures,

                                          CustomData

                                      );

                if (CustomGetPWMValueResponseParser is not null)
                    GetPWMValueResponse = CustomGetPWMValueResponseParser(JSON,
                                                                          GetPWMValueResponse);

                return true;

            }
            catch (Exception e)
            {
                GetPWMValueResponse  = null;
                ErrorResponse        = "The given JSON representation of a GetPWMValue response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetPWMValueResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetPWMValueResponseSerializer">A delegate to serialize custom GetPWMValue responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetPWMValueResponse>?  CustomGetPWMValueResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?              CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",        Status.ToString()),

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetPWMValueResponseSerializer is not null
                       ? CustomGetPWMValueResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetPWMValue failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetPWMValue request leading to this response.</param>
        public static GetPWMValueResponse RequestError(GetPWMValueRequest       Request,
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
                   GenericStatus.Rejected,
                   Result.FromErrorResponse(
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
        /// The GetPWMValue failed.
        /// </summary>
        /// <param name="Request">The GetPWMValue request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetPWMValueResponse FormationViolation(GetPWMValueRequest  Request,
                                                             String              ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FormationViolation(
                                              $"Invalid data format: {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The GetPWMValue failed.
        /// </summary>
        /// <param name="Request">The GetPWMValue request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetPWMValueResponse SignatureError(GetPWMValueRequest  Request,
                                                         String              ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.SignatureError(
                                              $"Invalid signature(s): {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The GetPWMValue failed.
        /// </summary>
        /// <param name="Request">The GetPWMValue request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetPWMValueResponse Failed(GetPWMValueRequest  Request,
                                                 String?             Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.Server(Description),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The GetPWMValue failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetPWMValue request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetPWMValueResponse ExceptionOccurred(GetPWMValueRequest  Request,
                                                            Exception           Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FromException(Exception),
                    SerializationFormat:  Request.SerializationFormat);

        #endregion


        #region Operator overloading

        #region Operator == (GetPWMValueResponse1, GetPWMValueResponse2)

        /// <summary>
        /// Compares two GetPWMValue responses for equality.
        /// </summary>
        /// <param name="GetPWMValueResponse1">A GetPWMValue response.</param>
        /// <param name="GetPWMValueResponse2">Another GetPWMValue response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetPWMValueResponse? GetPWMValueResponse1,
                                           GetPWMValueResponse? GetPWMValueResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetPWMValueResponse1, GetPWMValueResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetPWMValueResponse1 is null || GetPWMValueResponse2 is null)
                return false;

            return GetPWMValueResponse1.Equals(GetPWMValueResponse2);

        }

        #endregion

        #region Operator != (GetPWMValueResponse1, GetPWMValueResponse2)

        /// <summary>
        /// Compares two GetPWMValue responses for inequality.
        /// </summary>
        /// <param name="GetPWMValueResponse1">A GetPWMValue response.</param>
        /// <param name="GetPWMValueResponse2">Another GetPWMValue response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetPWMValueResponse? GetPWMValueResponse1,
                                           GetPWMValueResponse? GetPWMValueResponse2)

            => !(GetPWMValueResponse1 == GetPWMValueResponse2);

        #endregion

        #endregion

        #region IEquatable<GetPWMValueResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetPWMValue responses for equality.
        /// </summary>
        /// <param name="Object">A GetPWMValue response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetPWMValueResponse getPWMValueResponse &&
                   Equals(getPWMValueResponse);

        #endregion

        #region Equals(GetPWMValueResponse)

        /// <summary>
        /// Compares two GetPWMValue responses for equality.
        /// </summary>
        /// <param name="GetPWMValueResponse">A GetPWMValue response to compare with.</param>
        public override Boolean Equals(GetPWMValueResponse? GetPWMValueResponse)

            => GetPWMValueResponse is not null &&
               Status.Equals(GetPWMValueResponse.Status);

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

            => Status.AsText();

        #endregion

    }

}
