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
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The NotifyReport response.
    /// </summary>
    public class NotifyReportResponse : AResponse<NotifyReportRequest,
                                                  NotifyReportResponse>,
                                        IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyReportResponse");

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
        /// Create a new NotifyReport response.
        /// </summary>
        /// <param name="Request">The NotifyReport request leading to this response.</param>
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
        public NotifyReportResponse(NotifyReportRequest      Request,

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


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:NotifyReportResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a NotifyReport response.
        /// </summary>
        /// <param name="Request">The NotifyReport request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyReportResponseParser">A delegate to parse custom NotifyReport responses.</param>
        public static NotifyReportResponse Parse(NotifyReportRequest                                 Request,
                                                 JObject                                             JSON,
                                                 SourceRouting                                       Destination,
                                                 NetworkPath                                         NetworkPath,
                                                 DateTimeOffset?                                     ResponseTimestamp                  = null,
                                                 CustomJObjectParserDelegate<NotifyReportResponse>?  CustomNotifyReportResponseParser   = null,
                                                 CustomJObjectParserDelegate<Signature>?             CustomSignatureParser              = null,
                                                 CustomJObjectParserDelegate<CustomData>?            CustomCustomDataParser             = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var notifyReportResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyReportResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifyReportResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyReport response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out NotifyReportResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyReport response.
        /// </summary>
        /// <param name="Request">The NotifyReport request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyReportResponse">The parsed NotifyReport response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyReportResponseParser">A delegate to parse custom NotifyReport responses.</param>
        public static Boolean TryParse(NotifyReportRequest                                 Request,
                                       JObject                                             JSON,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out NotifyReportResponse?      NotifyReportResponse,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       DateTimeOffset?                                     ResponseTimestamp                  = null,
                                       CustomJObjectParserDelegate<NotifyReportResponse>?  CustomNotifyReportResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?             CustomSignatureParser              = null,
                                       CustomJObjectParserDelegate<CustomData>?            CustomCustomDataParser             = null)
        {

            ErrorResponse = null;

            try
            {

                NotifyReportResponse = null;

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


                NotifyReportResponse = new NotifyReportResponse(

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

                if (CustomNotifyReportResponseParser is not null)
                    NotifyReportResponse = CustomNotifyReportResponseParser(JSON,
                                                                            NotifyReportResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyReportResponse  = null;
                ErrorResponse         = "The given JSON representation of a NotifyReport response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyReportResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyReportResponseSerializer">A delegate to serialize custom NotifyReport responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                 IncludeJSONLDContext                   = false,
                              CustomJObjectSerializerDelegate<NotifyReportResponse>?  CustomNotifyReportResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
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

            return CustomNotifyReportResponseSerializer is not null
                       ? CustomNotifyReportResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyReport failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyReport request.</param>
        public static NotifyReportResponse RequestError(NotifyReportRequest      Request,
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
        /// The NotifyReport failed.
        /// </summary>
        /// <param name="Request">The NotifyReport request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyReportResponse FormationViolation(NotifyReportRequest  Request,
                                                              String               ErrorDescription)

            => new (Request,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyReport failed.
        /// </summary>
        /// <param name="Request">The NotifyReport request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyReportResponse SignatureError(NotifyReportRequest  Request,
                                                          String               ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyReport failed.
        /// </summary>
        /// <param name="Request">The NotifyReport request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyReportResponse Failed(NotifyReportRequest  Request,
                                                  String?              Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The NotifyReport failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyReport request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyReportResponse ExceptionOccurred(NotifyReportRequest  Request,
                                                            Exception            Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyReportResponse1, NotifyReportResponse2)

        /// <summary>
        /// Compares two NotifyReport responses for equality.
        /// </summary>
        /// <param name="NotifyReportResponse1">A NotifyReport response.</param>
        /// <param name="NotifyReportResponse2">Another NotifyReport response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyReportResponse? NotifyReportResponse1,
                                           NotifyReportResponse? NotifyReportResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyReportResponse1, NotifyReportResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyReportResponse1 is null || NotifyReportResponse2 is null)
                return false;

            return NotifyReportResponse1.Equals(NotifyReportResponse2);

        }

        #endregion

        #region Operator != (NotifyReportResponse1, NotifyReportResponse2)

        /// <summary>
        /// Compares two NotifyReport responses for inequality.
        /// </summary>
        /// <param name="NotifyReportResponse1">A NotifyReport response.</param>
        /// <param name="NotifyReportResponse2">Another NotifyReport response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyReportResponse? NotifyReportResponse1,
                                           NotifyReportResponse? NotifyReportResponse2)

            => !(NotifyReportResponse1 == NotifyReportResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyReportResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyReport responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyReport response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyReportResponse notifyReportResponse &&
                   Equals(notifyReportResponse);

        #endregion

        #region Equals(NotifyReportResponse)

        /// <summary>
        /// Compares two NotifyReport responses for equality.
        /// </summary>
        /// <param name="NotifyReportResponse">A NotifyReport response to compare with.</param>
        public override Boolean Equals(NotifyReportResponse? NotifyReportResponse)

            => NotifyReportResponse is not null &&
                   base.GenericEquals(NotifyReportResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "NotifyReportResponse";

        #endregion

    }

}
