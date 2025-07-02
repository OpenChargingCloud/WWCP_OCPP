﻿/*
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
    /// A SetErrorState response.
    /// </summary>
    public class SetErrorStateResponse : AResponse<SetErrorStateRequest,
                                                   SetErrorStateResponse>,
                                         IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/setErrorStateResponse");

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
        /// Create a new SetErrorState response.
        /// </summary>
        /// <param name="Request">The SetErrorState request leading to this response.</param>
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
        public SetErrorStateResponse(SetErrorStateRequest     Request,
                                     GenericStatus            Status,

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
        /// Parse the given JSON representation of a SetErrorState response.
        /// </summary>
        /// <param name="Request">The SetErrorState request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomSetErrorStateResponseParser">An optional delegate to parse custom SetErrorState responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static SetErrorStateResponse Parse(SetErrorStateRequest                                 Request,
                                                  JObject                                              JSON,
                                                  SourceRouting                                        Destination,
                                                  NetworkPath                                          NetworkPath,
                                                  DateTime?                                            ResponseTimestamp                   = null,
                                                  CustomJObjectParserDelegate<SetErrorStateResponse>?  CustomSetErrorStateResponseParser   = null,
                                                  CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                                  CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var setErrorStateResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSetErrorStateResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setErrorStateResponse;
            }

            throw new ArgumentException("The given JSON representation of a SetErrorState response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out SetErrorStateResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SetErrorState response.
        /// </summary>
        /// <param name="Request">The SetErrorState request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="SetErrorStateResponse">The parsed SetErrorState response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomSetErrorStateResponseParser">An optional delegate to parse custom SetErrorState responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(SetErrorStateRequest                                 Request,
                                       JObject                                              JSON,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out SetErrorStateResponse?      SetErrorStateResponse,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       DateTime?                                            ResponseTimestamp                   = null,
                                       CustomJObjectParserDelegate<SetErrorStateResponse>?  CustomSetErrorStateResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                       CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            try
            {

                SetErrorStateResponse = null;

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


                SetErrorStateResponse = new SetErrorStateResponse(

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

                if (CustomSetErrorStateResponseParser is not null)
                    SetErrorStateResponse = CustomSetErrorStateResponseParser(JSON,
                                                                              SetErrorStateResponse);

                return true;

            }
            catch (Exception e)
            {
                SetErrorStateResponse  = null;
                ErrorResponse          = "The given JSON representation of a SetErrorState response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetErrorStateResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetErrorStateResponseSerializer">A delegate to serialize custom SetErrorState responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetErrorStateResponse>?  CustomSetErrorStateResponseSerializer   = null,
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

            return CustomSetErrorStateResponseSerializer is not null
                       ? CustomSetErrorStateResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SetErrorState failed because of a request error.
        /// </summary>
        /// <param name="Request">The SetErrorState request leading to this response.</param>
        public static SetErrorStateResponse RequestError(SetErrorStateRequest     Request,
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
        /// The SetErrorState failed.
        /// </summary>
        /// <param name="Request">The SetErrorState request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetErrorStateResponse FormationViolation(SetErrorStateRequest  Request,
                                                               String                ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FormationViolation(
                                              $"Invalid data format: {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The SetErrorState failed.
        /// </summary>
        /// <param name="Request">The SetErrorState request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetErrorStateResponse SignatureError(SetErrorStateRequest  Request,
                                                           String                ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.SignatureError(
                                              $"Invalid signature(s): {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The SetErrorState failed.
        /// </summary>
        /// <param name="Request">The SetErrorState request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SetErrorStateResponse Failed(SetErrorStateRequest  Request,
                                                   String?               Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.Server(Description),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The SetErrorState failed because of an exception.
        /// </summary>
        /// <param name="Request">The SetErrorState request.</param>
        /// <param name="Exception">The exception.</param>
        public static SetErrorStateResponse ExceptionOccurred(SetErrorStateRequest  Request,
                                                              Exception             Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FromException(Exception),
                    SerializationFormat:  Request.SerializationFormat);

        #endregion


        #region Operator overloading

        #region Operator == (SetErrorStateResponse1, SetErrorStateResponse2)

        /// <summary>
        /// Compares two SetErrorState responses for equality.
        /// </summary>
        /// <param name="SetErrorStateResponse1">A SetErrorState response.</param>
        /// <param name="SetErrorStateResponse2">Another SetErrorState response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetErrorStateResponse? SetErrorStateResponse1,
                                           SetErrorStateResponse? SetErrorStateResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetErrorStateResponse1, SetErrorStateResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetErrorStateResponse1 is null || SetErrorStateResponse2 is null)
                return false;

            return SetErrorStateResponse1.Equals(SetErrorStateResponse2);

        }

        #endregion

        #region Operator != (SetErrorStateResponse1, SetErrorStateResponse2)

        /// <summary>
        /// Compares two SetErrorState responses for inequality.
        /// </summary>
        /// <param name="SetErrorStateResponse1">A SetErrorState response.</param>
        /// <param name="SetErrorStateResponse2">Another SetErrorState response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetErrorStateResponse? SetErrorStateResponse1,
                                           SetErrorStateResponse? SetErrorStateResponse2)

            => !(SetErrorStateResponse1 == SetErrorStateResponse2);

        #endregion

        #endregion

        #region IEquatable<SetErrorStateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetErrorState responses for equality.
        /// </summary>
        /// <param name="Object">A SetErrorState response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetErrorStateResponse setErrorStateResponse &&
                   Equals(setErrorStateResponse);

        #endregion

        #region Equals(SetErrorStateResponse)

        /// <summary>
        /// Compares two SetErrorState responses for equality.
        /// </summary>
        /// <param name="SetErrorStateResponse">A SetErrorState response to compare with.</param>
        public override Boolean Equals(SetErrorStateResponse? SetErrorStateResponse)

            => SetErrorStateResponse is not null &&
               Status.Equals(SetErrorStateResponse.Status);

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
