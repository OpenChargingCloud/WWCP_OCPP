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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The NotifyPriorityCharging response.
    /// </summary>
    public class NotifyPriorityChargingResponse : AResponse<NotifyPriorityChargingRequest,
                                                            NotifyPriorityChargingResponse>,
                                                  IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyPriorityChargingResponse");

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
        /// Create a new NotifyPriorityCharging response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
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
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public NotifyPriorityChargingResponse(NotifyPriorityChargingRequest  Request,

                                              Result?                        Result                = null,
                                              DateTime?                      ResponseTimestamp     = null,

                                              SourceRouting?                 Destination           = null,
                                              NetworkPath?                   NetworkPath           = null,

                                              IEnumerable<KeyPair>?          SignKeys              = null,
                                              IEnumerable<SignInfo>?         SignInfos             = null,
                                              IEnumerable<Signature>?        Signatures            = null,

                                              CustomData?                    CustomData            = null,

                                              SerializationFormats?          SerializationFormat   = null,
                                              CancellationToken              CancellationToken     = default)

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

        {

            unchecked
            {
                hashCode = base.GetHashCode();
            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomNotifyPriorityChargingResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a NotifyPriorityCharging response.
        /// </summary>
        /// <param name="Request">The NotifyPriorityCharging request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyPriorityChargingResponseParser">A delegate to parse custom NotifyPriorityCharging responses.</param>
        public static NotifyPriorityChargingResponse Parse(NotifyPriorityChargingRequest                                 Request,
                                                           JObject                                                       JSON,
                                                           SourceRouting                                             Destination,
                                                           NetworkPath                                                   NetworkPath,
                                                           DateTime?                                                     ResponseTimestamp                            = null,
                                                           CustomJObjectParserDelegate<NotifyPriorityChargingResponse>?  CustomNotifyPriorityChargingResponseParser   = null,
                                                           CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                                           CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var notifyPriorityChargingResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyPriorityChargingResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifyPriorityChargingResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyPriorityCharging response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyPriorityChargingResponse, out ErrorResponse, CustomNotifyPriorityChargingResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyPriorityCharging response.
        /// </summary>
        /// <param name="Request">The NotifyPriorityCharging request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyPriorityChargingResponse">The parsed NotifyPriorityCharging response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyPriorityChargingResponseParser">A delegate to parse custom NotifyPriorityCharging responses.</param>
        public static Boolean TryParse(NotifyPriorityChargingRequest                                 Request,
                                       JObject                                                       JSON,
                                       SourceRouting                                             Destination,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out NotifyPriorityChargingResponse?      NotifyPriorityChargingResponse,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       DateTime?                                                     ResponseTimestamp                            = null,
                                       CustomJObjectParserDelegate<NotifyPriorityChargingResponse>?  CustomNotifyPriorityChargingResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                       CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            ErrorResponse = null;

            try
            {

                NotifyPriorityChargingResponse = null;

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


                NotifyPriorityChargingResponse = new NotifyPriorityChargingResponse(

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

                if (CustomNotifyPriorityChargingResponseParser is not null)
                    NotifyPriorityChargingResponse = CustomNotifyPriorityChargingResponseParser(JSON,
                                                                                                NotifyPriorityChargingResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyPriorityChargingResponse  = null;
                ErrorResponse                   = "The given JSON representation of a NotifyPriorityCharging response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyPriorityChargingResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyPriorityChargingResponseSerializer">A delegate to serialize custom NotifyPriorityCharging responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyPriorityChargingResponse>?  CustomNotifyPriorityChargingResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
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

            return CustomNotifyPriorityChargingResponseSerializer is not null
                       ? CustomNotifyPriorityChargingResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyPriorityCharging failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyPriorityCharging request.</param>
        public static NotifyPriorityChargingResponse RequestError(NotifyPriorityChargingRequest  Request,
                                                                  EventTracking_Id               EventTrackingId,
                                                                  ResultCode                     ErrorCode,
                                                                  String?                        ErrorDescription    = null,
                                                                  JObject?                       ErrorDetails        = null,
                                                                  DateTime?                      ResponseTimestamp   = null,

                                                                  SourceRouting?                 Destination         = null,
                                                                  NetworkPath?                   NetworkPath         = null,

                                                                  IEnumerable<KeyPair>?          SignKeys            = null,
                                                                  IEnumerable<SignInfo>?         SignInfos           = null,
                                                                  IEnumerable<Signature>?        Signatures          = null,

                                                                  CustomData?                    CustomData          = null)

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
        /// The NotifyPriorityCharging failed.
        /// </summary>
        /// <param name="Request">The NotifyPriorityCharging request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyPriorityChargingResponse FormationViolation(NotifyPriorityChargingRequest  Request,
                                                                        String                         ErrorDescription)

            => new (Request,
                   OCPPv2_1.Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyPriorityCharging failed.
        /// </summary>
        /// <param name="Request">The NotifyPriorityCharging request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyPriorityChargingResponse SignatureError(NotifyPriorityChargingRequest  Request,
                                                                    String                         ErrorDescription)

            => new (Request,
                   OCPPv2_1.Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyPriorityCharging failed.
        /// </summary>
        /// <param name="Request">The NotifyPriorityCharging request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyPriorityChargingResponse Failed(NotifyPriorityChargingRequest  Request,
                                                            String?                        Description   = null)

            => new (Request,
                    OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The NotifyPriorityCharging failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyPriorityCharging request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyPriorityChargingResponse ExceptionOccured(NotifyPriorityChargingRequest  Request,
                                                                      Exception                      Exception)

            => new (Request,
                    OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyPriorityChargingResponse1, NotifyPriorityChargingResponse2)

        /// <summary>
        /// Compares two NotifyPriorityCharging responses for equality.
        /// </summary>
        /// <param name="NotifyPriorityChargingResponse1">A NotifyPriorityCharging response.</param>
        /// <param name="NotifyPriorityChargingResponse2">Another NotifyPriorityCharging response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyPriorityChargingResponse? NotifyPriorityChargingResponse1,
                                           NotifyPriorityChargingResponse? NotifyPriorityChargingResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyPriorityChargingResponse1, NotifyPriorityChargingResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyPriorityChargingResponse1 is null || NotifyPriorityChargingResponse2 is null)
                return false;

            return NotifyPriorityChargingResponse1.Equals(NotifyPriorityChargingResponse2);

        }

        #endregion

        #region Operator != (NotifyPriorityChargingResponse1, NotifyPriorityChargingResponse2)

        /// <summary>
        /// Compares two NotifyPriorityCharging responses for inequality.
        /// </summary>
        /// <param name="NotifyPriorityChargingResponse1">A NotifyPriorityCharging response.</param>
        /// <param name="NotifyPriorityChargingResponse2">Another NotifyPriorityCharging response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyPriorityChargingResponse? NotifyPriorityChargingResponse1,
                                           NotifyPriorityChargingResponse? NotifyPriorityChargingResponse2)

            => !(NotifyPriorityChargingResponse1 == NotifyPriorityChargingResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyPriorityChargingResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyPriorityCharging responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyPriorityCharging response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyPriorityChargingResponse notifyPriorityChargingResponse &&
                   Equals(notifyPriorityChargingResponse);

        #endregion

        #region Equals(NotifyPriorityChargingResponse)

        /// <summary>
        /// Compares two NotifyPriorityCharging responses for equality.
        /// </summary>
        /// <param name="NotifyPriorityChargingResponse">A NotifyPriorityCharging response to compare with.</param>
        public override Boolean Equals(NotifyPriorityChargingResponse? NotifyPriorityChargingResponse)

            => NotifyPriorityChargingResponse is not null &&
                   base.GenericEquals(NotifyPriorityChargingResponse);

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

            => "NotifyPriorityChargingResponse";

        #endregion

    }

}
