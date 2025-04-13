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
    /// The ExtendedTriggerMessage response.
    /// </summary>
    [SecurityExtensions]
    public class ExtendedTriggerMessageResponse : AResponse<ExtendedTriggerMessageRequest,
                                                            ExtendedTriggerMessageResponse>,
                                                  IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/extendedTriggerMessageResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext         Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the ExtendedTriggerMessage command.
        /// </summary>
        public TriggerMessageStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ExtendedTriggerMessage response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="Status">The success or failure of the trigger message command.</param>
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
        public ExtendedTriggerMessageResponse(ExtendedTriggerMessageRequest  Request,
                                              TriggerMessageStatus           Status,

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

            this.Status = Status;

            unchecked
            {

                hashCode = this.Status.GetHashCode() * 3 ^
                           base.       GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:extendedTriggerMessageResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:extendedTriggerMessageResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ExtendedTriggerMessageResponse",
        //     "title":   "ExtendedTriggerMessageResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
        //                 "NotImplemented"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an ExtendedTriggerMessage response.
        /// </summary>
        /// <param name="Request">The ExtendedTriggerMessage request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomExtendedTriggerMessageResponseParser">An optional delegate to parse custom ExtendedTriggerMessage responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static ExtendedTriggerMessageResponse Parse(ExtendedTriggerMessageRequest                                 Request,
                                                           JObject                                                       JSON,
                                                           SourceRouting                                                 Destination,
                                                           NetworkPath                                                   NetworkPath,
                                                           DateTime?                                                     ResponseTimestamp                            = null,
                                                           CustomJObjectParserDelegate<ExtendedTriggerMessageResponse>?  CustomExtendedTriggerMessageResponseParser   = null,
                                                           CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                                           CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var extendedTriggerMessageResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomExtendedTriggerMessageResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return extendedTriggerMessageResponse;
            }

            throw new ArgumentException("The given JSON representation of an ExtendedTriggerMessage response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out ExtendedTriggerMessageResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an ExtendedTriggerMessage response.
        /// </summary>
        /// <param name="Request">The ExtendedTriggerMessage request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ExtendedTriggerMessageResponse">The parsed ExtendedTriggerMessage response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomExtendedTriggerMessageResponseParser">An optional delegate to parse custom ExtendedTriggerMessage responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(ExtendedTriggerMessageRequest                                 Request,
                                       JObject                                                       JSON,
                                       SourceRouting                                                 Destination,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out ExtendedTriggerMessageResponse?      ExtendedTriggerMessageResponse,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       DateTime?                                                     ResponseTimestamp                            = null,
                                       CustomJObjectParserDelegate<ExtendedTriggerMessageResponse>?  CustomExtendedTriggerMessageResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                       CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            try
            {

                ExtendedTriggerMessageResponse = null;

                #region TriggerMessageStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "trigger message status",
                                       TriggerMessageStatusExtensions.Parse,
                                       out TriggerMessageStatus TriggerMessageStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures                      [optional, OCPP_CSE]

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

                #region CustomData                      [optional]

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


                ExtendedTriggerMessageResponse = new ExtendedTriggerMessageResponse(

                                                     Request,
                                                     TriggerMessageStatus,

                                                     null,
                                                     ResponseTimestamp,

                                                     Destination,
                                                     NetworkPath,

                                                     null,
                                                     null,
                                                     Signatures,

                                                     CustomData

                                                 );

                if (CustomExtendedTriggerMessageResponseParser is not null)
                    ExtendedTriggerMessageResponse = CustomExtendedTriggerMessageResponseParser(JSON,
                                                                                                ExtendedTriggerMessageResponse);

                return true;

            }
            catch (Exception e)
            {
                ExtendedTriggerMessageResponse  = null;
                ErrorResponse                   = "The given JSON representation of an ExtendedTriggerMessage response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomExtendedTriggerMessageResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomExtendedTriggerMessageResponseSerializer">A delegate to serialize custom ExtendedTriggerMessage responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ExtendedTriggerMessageResponse>?  CustomExtendedTriggerMessageResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomExtendedTriggerMessageResponseSerializer is not null
                       ? CustomExtendedTriggerMessageResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ExtendedTriggerMessage failed because of a request error.
        /// </summary>
        /// <param name="Request">The ExtendedTriggerMessage request.</param>
        public static ExtendedTriggerMessageResponse RequestError(ExtendedTriggerMessageRequest  Request,
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
                   TriggerMessageStatus.Rejected,
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
        /// The ExtendedTriggerMessage failed.
        /// </summary>
        /// <param name="Request">The ExtendedTriggerMessage request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ExtendedTriggerMessageResponse FormationViolation(ExtendedTriggerMessageRequest  Request,
                                                                        String                         ErrorDescription)

            => new (Request,
                    TriggerMessageStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The ExtendedTriggerMessage failed.
        /// </summary>
        /// <param name="Request">The ExtendedTriggerMessage request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ExtendedTriggerMessageResponse SignatureError(ExtendedTriggerMessageRequest  Request,
                                                                    String                         ErrorDescription)

            => new (Request,
                    TriggerMessageStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The ExtendedTriggerMessage failed.
        /// </summary>
        /// <param name="Request">The ExtendedTriggerMessage request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ExtendedTriggerMessageResponse Failed(ExtendedTriggerMessageRequest  Request,
                                                            String?                        Description   = null)

            => new (Request,
                    TriggerMessageStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The ExtendedTriggerMessage failed because of an exception.
        /// </summary>
        /// <param name="Request">The ExtendedTriggerMessage request.</param>
        /// <param name="Exception">The exception.</param>
        public static ExtendedTriggerMessageResponse ExceptionOccurred(ExtendedTriggerMessageRequest  Request,
                                                                      Exception                      Exception)

            => new (Request,
                    TriggerMessageStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ExtendedTriggerMessageResponse1, ExtendedTriggerMessageResponse2)

        /// <summary>
        /// Compares two ExtendedTriggerMessage responses for equality.
        /// </summary>
        /// <param name="ExtendedTriggerMessageResponse1">A ExtendedTriggerMessage response.</param>
        /// <param name="ExtendedTriggerMessageResponse2">Another ExtendedTriggerMessage response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ExtendedTriggerMessageResponse? ExtendedTriggerMessageResponse1,
                                           ExtendedTriggerMessageResponse? ExtendedTriggerMessageResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ExtendedTriggerMessageResponse1, ExtendedTriggerMessageResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ExtendedTriggerMessageResponse1 is null || ExtendedTriggerMessageResponse2 is null)
                return false;

            return ExtendedTriggerMessageResponse1.Equals(ExtendedTriggerMessageResponse2);

        }

        #endregion

        #region Operator != (ExtendedTriggerMessageResponse1, ExtendedTriggerMessageResponse2)

        /// <summary>
        /// Compares two ExtendedTriggerMessage responses for inequality.
        /// </summary>
        /// <param name="ExtendedTriggerMessageResponse1">A ExtendedTriggerMessage response.</param>
        /// <param name="ExtendedTriggerMessageResponse2">Another ExtendedTriggerMessage response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ExtendedTriggerMessageResponse?ExtendedTriggerMessageResponse1,
                                           ExtendedTriggerMessageResponse?ExtendedTriggerMessageResponse2)

            => !(ExtendedTriggerMessageResponse1 == ExtendedTriggerMessageResponse2);

        #endregion

        #endregion

        #region IEquatable<ExtendedTriggerMessageResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ExtendedTriggerMessage responses for equality.
        /// </summary>
        /// <param name="Object">An ExtendedTriggerMessage response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ExtendedTriggerMessageResponse extendedTriggerMessageResponse &&
                   Equals(extendedTriggerMessageResponse);

        #endregion

        #region Equals(ExtendedTriggerMessageResponse)

        /// <summary>
        /// Compares two ExtendedTriggerMessage responses for equality.
        /// </summary>
        /// <param name="ExtendedTriggerMessageResponse">An ExtendedTriggerMessage response to compare with.</param>
        public override Boolean Equals(ExtendedTriggerMessageResponse? ExtendedTriggerMessageResponse)

            => ExtendedTriggerMessageResponse is not null &&
                   Status.Equals(ExtendedTriggerMessageResponse.Status);

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

            => Status.ToString();

        #endregion

    }

}
