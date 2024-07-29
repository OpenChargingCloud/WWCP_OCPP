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
    /// The GetTransactionStatus response.
    /// </summary>
    public class GetTransactionStatusResponse : AResponse<CSMS.GetTransactionStatusRequest,
                                                          GetTransactionStatusResponse>,
                                                IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getTransactionStatusResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Whether there are still message to be delivered.
        /// </summary>
        [Mandatory]
        public Boolean        MessagesInQueue     { get; }

        /// <summary>
        /// The optional indication whether the transaction is still ongoing.
        /// </summary>
        [Optional]
        public Boolean?       OngoingIndicator    { get; }

        #endregion

        #region Constructor(s)

        #region GetTransactionStatusResponse(Request, MessagesInQueue, OngoingIndicator = null, ...)

        /// <summary>
        /// Create a new GetTransactionStatus response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="MessagesInQueue">Whether there are still message to be delivered.</param>
        /// <param name="OngoingIndicator">An optional indication whether the transaction is still ongoing.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetTransactionStatusResponse(CSMS.GetTransactionStatusRequest  Request,
                                            Boolean                           MessagesInQueue,
                                            Boolean?                          OngoingIndicator    = null,
                                            DateTime?                         ResponseTimestamp   = null,

                                            IEnumerable<KeyPair>?             SignKeys            = null,
                                            IEnumerable<SignInfo>?            SignInfos           = null,
                                            IEnumerable<Signature>?           Signatures          = null,

                                            CustomData?                       CustomData          = null)

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

            this.MessagesInQueue   = MessagesInQueue;
            this.OngoingIndicator  = OngoingIndicator;

        }

        #endregion

        #region GetTransactionStatusResponse(Request, Result)

        /// <summary>
        /// Create a new GetTransactionStatus response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetTransactionStatusResponse(CSMS.GetTransactionStatusRequest  Request,
                                            Result                            Result,
                                            DateTime?                         ResponseTimestamp   = null,

                                            NetworkingNode_Id?                DestinationId       = null,
                                            NetworkPath?                      NetworkPath         = null,

                                            IEnumerable<KeyPair>?             SignKeys            = null,
                                            IEnumerable<SignInfo>?            SignInfos           = null,
                                            IEnumerable<Signature>?           Signatures          = null,

                                            CustomData?                       CustomData          = null)

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


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetTransactionStatusResponse",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "ongoingIndicator": {
        //       "description": "Whether the transaction is still ongoing.",
        //       "type": "boolean"
        //     },
        //     "messagesInQueue": {
        //       "description": "Whether there are still message to be delivered.",
        //       "type": "boolean"
        //     }
        //   },
        //   "required": [
        //     "messagesInQueue"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetTransactionStatusResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetTransactionStatus response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetTransactionStatusResponseParser">A delegate to parse custom GetTransactionStatus responses.</param>
        public static GetTransactionStatusResponse Parse(CSMS.GetTransactionStatusRequest                            Request,
                                                         JObject                                                     JSON,
                                                         CustomJObjectParserDelegate<GetTransactionStatusResponse>?  CustomGetTransactionStatusResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getTransactionStatusResponse,
                         out var errorResponse,
                         CustomGetTransactionStatusResponseParser))
            {
                return getTransactionStatusResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetTransactionStatus response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetTransactionStatusResponse, out ErrorResponse, CustomGetTransactionStatusResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetTransactionStatus response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetTransactionStatusResponse">The parsed GetTransactionStatus response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetTransactionStatusResponseParser">A delegate to parse custom GetTransactionStatus responses.</param>
        public static Boolean TryParse(CSMS.GetTransactionStatusRequest                            Request,
                                       JObject                                                     JSON,
                                       [NotNullWhen(true)]  out GetTransactionStatusResponse?      GetTransactionStatusResponse,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       CustomJObjectParserDelegate<GetTransactionStatusResponse>?  CustomGetTransactionStatusResponseParser   = null)
        {

            try
            {

                GetTransactionStatusResponse = null;

                #region MessagesInQueue     [mandatory]

                if (!JSON.ParseMandatory("messagesInQueue",
                                         "messages in queue",
                                         out Boolean MessagesInQueue,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region OngoingIndicator    [optional]

                if (JSON.ParseOptional("ongoingIndicator",
                                       "ongoing indicator",
                                       out Boolean? OngoingIndicator,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures          [optional, OCPP_CSE]

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

                #region CustomData          [optional]

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


                GetTransactionStatusResponse = new GetTransactionStatusResponse(
                                                   Request,
                                                   MessagesInQueue,
                                                   OngoingIndicator,
                                                   null,
                                                   null,
                                                   null,
                                                   Signatures,
                                                   CustomData
                                               );

                if (CustomGetTransactionStatusResponseParser is not null)
                    GetTransactionStatusResponse = CustomGetTransactionStatusResponseParser(JSON,
                                                                                            GetTransactionStatusResponse);

                return true;

            }
            catch (Exception e)
            {
                GetTransactionStatusResponse  = null;
                ErrorResponse                 = "The given JSON representation of a GetTransactionStatus response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetTransactionStatusResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetTransactionStatusResponseSerializer">A delegate to serialize custom GetTransactionStatus responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetTransactionStatusResponse>?  CustomGetTransactionStatusResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("messagesInQueue",    MessagesInQueue),

                           OngoingIndicator is not null
                               ? new JProperty("ongoingIndicator",   OngoingIndicator)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetTransactionStatusResponseSerializer is not null
                       ? CustomGetTransactionStatusResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetTransactionStatus failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetTransactionStatus request.</param>
        public static GetTransactionStatusResponse RequestError(CSMS.GetTransactionStatusRequest  Request,
                                                                EventTracking_Id                  EventTrackingId,
                                                                ResultCode                        ErrorCode,
                                                                String?                           ErrorDescription    = null,
                                                                JObject?                          ErrorDetails        = null,
                                                                DateTime?                         ResponseTimestamp   = null,

                                                                NetworkingNode_Id?                DestinationId       = null,
                                                                NetworkPath?                      NetworkPath         = null,

                                                                IEnumerable<KeyPair>?             SignKeys            = null,
                                                                IEnumerable<SignInfo>?            SignInfos           = null,
                                                                IEnumerable<Signature>?           Signatures          = null,

                                                                CustomData?                       CustomData          = null)

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
        /// The GetTransactionStatus failed.
        /// </summary>
        /// <param name="Request">The GetTransactionStatus request.</param>
        /// <param name="ErrorDescription">An optional error decription.</param>
        public static GetTransactionStatusResponse SignatureError(CSMS.GetTransactionStatusRequest  Request,
                                                                  String                            ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The GetTransactionStatus failed.
        /// </summary>
        /// <param name="Request">The GetTransactionStatus request.</param>
        /// <param name="Description">An optional error decription.</param>
        public static GetTransactionStatusResponse Failed(CSMS.GetTransactionStatusRequest  Request,
                                                          String?                           Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The GetTransactionStatus failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetTransactionStatus request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetTransactionStatusResponse ExceptionOccured(CSMS.GetTransactionStatusRequest  Request,
                                                                    Exception                         Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetTransactionStatusResponse1, GetTransactionStatusResponse2)

        /// <summary>
        /// Compares two GetTransactionStatus responses for equality.
        /// </summary>
        /// <param name="GetTransactionStatusResponse1">A GetTransactionStatus response.</param>
        /// <param name="GetTransactionStatusResponse2">Another GetTransactionStatus response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetTransactionStatusResponse? GetTransactionStatusResponse1,
                                           GetTransactionStatusResponse? GetTransactionStatusResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetTransactionStatusResponse1, GetTransactionStatusResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetTransactionStatusResponse1 is null || GetTransactionStatusResponse2 is null)
                return false;

            return GetTransactionStatusResponse1.Equals(GetTransactionStatusResponse2);

        }

        #endregion

        #region Operator != (GetTransactionStatusResponse1, GetTransactionStatusResponse2)

        /// <summary>
        /// Compares two GetTransactionStatus responses for inequality.
        /// </summary>
        /// <param name="GetTransactionStatusResponse1">A GetTransactionStatus response.</param>
        /// <param name="GetTransactionStatusResponse2">Another GetTransactionStatus response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetTransactionStatusResponse? GetTransactionStatusResponse1,
                                           GetTransactionStatusResponse? GetTransactionStatusResponse2)

            => !(GetTransactionStatusResponse1 == GetTransactionStatusResponse2);

        #endregion

        #endregion

        #region IEquatable<GetTransactionStatusResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetTransactionStatus responses for equality.
        /// </summary>
        /// <param name="Object">A GetTransactionStatus response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetTransactionStatusResponse getTransactionStatusResponse &&
                   Equals(getTransactionStatusResponse);

        #endregion

        #region Equals(GetTransactionStatusResponse)

        /// <summary>
        /// Compares two GetTransactionStatus responses for equality.
        /// </summary>
        /// <param name="GetTransactionStatusResponse">A GetTransactionStatus response to compare with.</param>
        public override Boolean Equals(GetTransactionStatusResponse? GetTransactionStatusResponse)

            => GetTransactionStatusResponse is not null &&

               MessagesInQueue.Equals(GetTransactionStatusResponse.MessagesInQueue) &&

            ((!OngoingIndicator.HasValue && !GetTransactionStatusResponse.OngoingIndicator.HasValue) ||
               OngoingIndicator.HasValue &&  GetTransactionStatusResponse.OngoingIndicator.HasValue && OngoingIndicator.Value.Equals(GetTransactionStatusResponse.OngoingIndicator.Value)) &&

               base.    GenericEquals(GetTransactionStatusResponse);

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

                return MessagesInQueue.  GetHashCode()       * 5 ^
                      (OngoingIndicator?.GetHashCode() ?? 0) * 3 ^

                       base.             GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   MessagesInQueue
                       ? "Messages in queue"
                       : "No messages in queue",

                   OngoingIndicator.HasValue
                       ? OngoingIndicator.Value
                             ? ", ongoing transaction"
                             : ", transaction finished"
                       : ""

               );

        #endregion

    }

}
