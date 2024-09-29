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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The NotifyCustomerInformation response.
    /// </summary>
    public class NotifyCustomerInformationResponse : AResponse<NotifyCustomerInformationRequest,
                                                               NotifyCustomerInformationResponse>,
                                                     IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyCustomerInformationResponse");

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
        /// Create a new NotifyCustomerInformation response.
        /// </summary>
        /// <param name="Request">The NotifyCustomerInformation request leading to this response.</param>
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
        public NotifyCustomerInformationResponse(NotifyCustomerInformationRequest  Request,

                                                 Result?                           Result                = null,
                                                 DateTime?                         ResponseTimestamp     = null,

                                                 SourceRouting?                    Destination           = null,
                                                 NetworkPath?                      NetworkPath           = null,

                                                 IEnumerable<KeyPair>?             SignKeys              = null,
                                                 IEnumerable<SignInfo>?            SignInfos             = null,
                                                 IEnumerable<Signature>?           Signatures            = null,

                                                 CustomData?                       CustomData            = null,

                                                 SerializationFormats?             SerializationFormat   = null,
                                                 CancellationToken                 CancellationToken     = default)

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


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyCustomerInformationResponse",
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
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomNotifyCustomerInformationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a NotifyCustomerInformation response.
        /// </summary>
        /// <param name="Request">The NotifyCustomerInformation request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyCustomerInformationResponseParser">A delegate to parse custom NotifyCustomerInformation responses.</param>
        public static NotifyCustomerInformationResponse Parse(NotifyCustomerInformationRequest                                 Request,
                                                              JObject                                                          JSON,
                                                              SourceRouting                                                Destination,
                                                              NetworkPath                                                      NetworkPath,
                                                              DateTime?                                                        ResponseTimestamp                               = null,
                                                              CustomJObjectParserDelegate<NotifyCustomerInformationResponse>?  CustomNotifyCustomerInformationResponseParser   = null,
                                                              CustomJObjectParserDelegate<Signature>?                          CustomSignatureParser                           = null,
                                                              CustomJObjectParserDelegate<CustomData>?                         CustomCustomDataParser                          = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var notifyCustomerInformationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyCustomerInformationResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifyCustomerInformationResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyCustomerInformation response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyCustomerInformationResponse, out ErrorResponse, CustomNotifyCustomerInformationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyCustomerInformation response.
        /// </summary>
        /// <param name="Request">The NotifyCustomerInformation request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyCustomerInformationResponse">The parsed NotifyCustomerInformation response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyCustomerInformationResponseParser">A delegate to parse custom NotifyCustomerInformation responses.</param>
        public static Boolean TryParse(NotifyCustomerInformationRequest                                 Request,
                                       JObject                                                          JSON,
                                       SourceRouting                                                Destination,
                                       NetworkPath                                                      NetworkPath,
                                       [NotNullWhen(true)]  out NotifyCustomerInformationResponse?      NotifyCustomerInformationResponse,
                                       [NotNullWhen(false)] out String?                                 ErrorResponse,
                                       DateTime?                                                        ResponseTimestamp                               = null,
                                       CustomJObjectParserDelegate<NotifyCustomerInformationResponse>?  CustomNotifyCustomerInformationResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                          CustomSignatureParser                           = null,
                                       CustomJObjectParserDelegate<CustomData>?                         CustomCustomDataParser                          = null)
        {

            ErrorResponse = null;

            try
            {

                NotifyCustomerInformationResponse = null;

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


                NotifyCustomerInformationResponse = new NotifyCustomerInformationResponse(

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

                if (CustomNotifyCustomerInformationResponseParser is not null)
                    NotifyCustomerInformationResponse = CustomNotifyCustomerInformationResponseParser(JSON,
                                                                                                      NotifyCustomerInformationResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyCustomerInformationResponse  = null;
                ErrorResponse                      = "The given JSON representation of a NotifyCustomerInformation response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyCustomerInformationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyCustomerInformationResponseSerializer">A delegate to serialize custom NotifyCustomerInformation responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyCustomerInformationResponse>?  CustomNotifyCustomerInformationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                          CustomSignatureSerializer                           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                         CustomCustomDataSerializer                          = null)
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

            return CustomNotifyCustomerInformationResponseSerializer is not null
                       ? CustomNotifyCustomerInformationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyCustomerInformation failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyCustomerInformation request.</param>
        public static NotifyCustomerInformationResponse RequestError(NotifyCustomerInformationRequest  Request,
                                                                     EventTracking_Id                  EventTrackingId,
                                                                     ResultCode                        ErrorCode,
                                                                     String?                           ErrorDescription    = null,
                                                                     JObject?                          ErrorDetails        = null,
                                                                     DateTime?                         ResponseTimestamp   = null,

                                                                     SourceRouting?                    Destination         = null,
                                                                     NetworkPath?                      NetworkPath         = null,

                                                                     IEnumerable<KeyPair>?             SignKeys            = null,
                                                                     IEnumerable<SignInfo>?            SignInfos           = null,
                                                                     IEnumerable<Signature>?           Signatures          = null,

                                                                     CustomData?                       CustomData          = null)

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
        /// The NotifyCustomerInformation failed.
        /// </summary>
        /// <param name="Request">The NotifyCustomerInformation request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyCustomerInformationResponse FormationViolation(NotifyCustomerInformationRequest  Request,
                                                                           String                            ErrorDescription)

            => new (Request,
                   OCPPv2_1.Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyCustomerInformation failed.
        /// </summary>
        /// <param name="Request">The NotifyCustomerInformation request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyCustomerInformationResponse SignatureError(NotifyCustomerInformationRequest  Request,
                                                                       String                            ErrorDescription)

            => new (Request,
                   OCPPv2_1.Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyCustomerInformation failed.
        /// </summary>
        /// <param name="Request">The NotifyCustomerInformation request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyCustomerInformationResponse Failed(NotifyCustomerInformationRequest  Request,
                                                               String?                           Description   = null)

            => new (Request,
                    OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The NotifyCustomerInformation failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyCustomerInformation request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyCustomerInformationResponse ExceptionOccured(NotifyCustomerInformationRequest  Request,
                                                                         Exception                         Exception)

            => new (Request,
                    OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyCustomerInformationResponse1, NotifyCustomerInformationResponse2)

        /// <summary>
        /// Compares two NotifyCustomerInformation responses for equality.
        /// </summary>
        /// <param name="NotifyCustomerInformationResponse1">A NotifyCustomerInformation response.</param>
        /// <param name="NotifyCustomerInformationResponse2">Another NotifyCustomerInformation response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyCustomerInformationResponse? NotifyCustomerInformationResponse1,
                                           NotifyCustomerInformationResponse? NotifyCustomerInformationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyCustomerInformationResponse1, NotifyCustomerInformationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyCustomerInformationResponse1 is null || NotifyCustomerInformationResponse2 is null)
                return false;

            return NotifyCustomerInformationResponse1.Equals(NotifyCustomerInformationResponse2);

        }

        #endregion

        #region Operator != (NotifyCustomerInformationResponse1, NotifyCustomerInformationResponse2)

        /// <summary>
        /// Compares two NotifyCustomerInformation responses for inequality.
        /// </summary>
        /// <param name="NotifyCustomerInformationResponse1">A NotifyCustomerInformation response.</param>
        /// <param name="NotifyCustomerInformationResponse2">Another NotifyCustomerInformation response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyCustomerInformationResponse? NotifyCustomerInformationResponse1,
                                           NotifyCustomerInformationResponse? NotifyCustomerInformationResponse2)

            => !(NotifyCustomerInformationResponse1 == NotifyCustomerInformationResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyCustomerInformationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyCustomerInformation responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyCustomerInformation response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyCustomerInformationResponse notifyCustomerInformationResponse &&
                   Equals(notifyCustomerInformationResponse);

        #endregion

        #region Equals(NotifyCustomerInformationResponse)

        /// <summary>
        /// Compares two NotifyCustomerInformation responses for equality.
        /// </summary>
        /// <param name="NotifyCustomerInformationResponse">A NotifyCustomerInformation response to compare with.</param>
        public override Boolean Equals(NotifyCustomerInformationResponse? NotifyCustomerInformationResponse)

            => NotifyCustomerInformationResponse is not null &&
                   base.GenericEquals(NotifyCustomerInformationResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "NotifyCustomerInformationResponse";

        #endregion

    }

}
