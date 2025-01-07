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
    /// The NotifyMonitoringReport response.
    /// </summary>
    public class NotifyMonitoringReportResponse : AResponse<NotifyMonitoringReportRequest,
                                                            NotifyMonitoringReportResponse>,
                                                  IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyMonitoringReportResponse");

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
        /// Create a new NotifyMonitoringReport response.
        /// </summary>
        /// <param name="Request">The NotifyMonitoringReport request leading to this response.</param>
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
        public NotifyMonitoringReportResponse(NotifyMonitoringReportRequest  Request,

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


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyMonitoringReportResponse",
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

        #region (static) Parse   (Request, JSON, CustomNotifyMonitoringReportResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a NotifyMonitoringReport response.
        /// </summary>
        /// <param name="Request">The NotifyMonitoringReport request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyMonitoringReportResponseParser">A delegate to parse custom NotifyMonitoringReport responses.</param>
        public static NotifyMonitoringReportResponse Parse(NotifyMonitoringReportRequest                                 Request,
                                                           JObject                                                       JSON,
                                                           SourceRouting                                             Destination,
                                                           NetworkPath                                                   NetworkPath,
                                                           DateTime?                                                     ResponseTimestamp                            = null,
                                                           CustomJObjectParserDelegate<NotifyMonitoringReportResponse>?  CustomNotifyMonitoringReportResponseParser   = null,
                                                           CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                                           CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var notifyMonitoringReportResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyMonitoringReportResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifyMonitoringReportResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyMonitoringReport response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyMonitoringReportResponse, out ErrorResponse, CustomNotifyMonitoringReportResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyMonitoringReport response.
        /// </summary>
        /// <param name="Request">The NotifyMonitoringReport request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyMonitoringReportResponse">The parsed NotifyMonitoringReport response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyMonitoringReportResponseParser">A delegate to parse custom NotifyMonitoringReport responses.</param>
        public static Boolean TryParse(NotifyMonitoringReportRequest                                 Request,
                                       JObject                                                       JSON,
                                       SourceRouting                                             Destination,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out NotifyMonitoringReportResponse?      NotifyMonitoringReportResponse,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       DateTime?                                                     ResponseTimestamp                            = null,
                                       CustomJObjectParserDelegate<NotifyMonitoringReportResponse>?  CustomNotifyMonitoringReportResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                       CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            ErrorResponse = null;

            try
            {

                NotifyMonitoringReportResponse = null;

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


                NotifyMonitoringReportResponse = new NotifyMonitoringReportResponse(

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

                if (CustomNotifyMonitoringReportResponseParser is not null)
                    NotifyMonitoringReportResponse = CustomNotifyMonitoringReportResponseParser(JSON,
                                                                                                NotifyMonitoringReportResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyMonitoringReportResponse  = null;
                ErrorResponse                   = "The given JSON representation of a NotifyMonitoringReport response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyMonitoringReportResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyMonitoringReportResponseSerializer">A delegate to serialize custom NotifyMonitoringReport responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyMonitoringReportResponse>?  CustomNotifyMonitoringReportResponseSerializer   = null,
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

            return CustomNotifyMonitoringReportResponseSerializer is not null
                       ? CustomNotifyMonitoringReportResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyMonitoringReport failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyMonitoringReport request.</param>
        public static NotifyMonitoringReportResponse RequestError(NotifyMonitoringReportRequest  Request,
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
        /// The NotifyMonitoringReport failed.
        /// </summary>
        /// <param name="Request">The NotifyMonitoringReport request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyMonitoringReportResponse FormationViolation(NotifyMonitoringReportRequest  Request,
                                                                        String                         ErrorDescription)

            => new (Request,
                   OCPPv2_1.Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyMonitoringReport failed.
        /// </summary>
        /// <param name="Request">The NotifyMonitoringReport request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyMonitoringReportResponse SignatureError(NotifyMonitoringReportRequest  Request,
                                                                    String                         ErrorDescription)

            => new (Request,
                   OCPPv2_1.Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyMonitoringReport failed.
        /// </summary>
        /// <param name="Request">The NotifyMonitoringReport request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyMonitoringReportResponse Failed(NotifyMonitoringReportRequest  Request,
                                                            String?                        Description   = null)

            => new (Request,
                    OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The NotifyMonitoringReport failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyMonitoringReport request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyMonitoringReportResponse ExceptionOccured(NotifyMonitoringReportRequest  Request,
                                                                      Exception                      Exception)

            => new (Request,
                    OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyMonitoringReportResponse1, NotifyMonitoringReportResponse2)

        /// <summary>
        /// Compares two NotifyMonitoringReport responses for equality.
        /// </summary>
        /// <param name="NotifyMonitoringReportResponse1">A NotifyMonitoringReport response.</param>
        /// <param name="NotifyMonitoringReportResponse2">Another NotifyMonitoringReport response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyMonitoringReportResponse? NotifyMonitoringReportResponse1,
                                           NotifyMonitoringReportResponse? NotifyMonitoringReportResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyMonitoringReportResponse1, NotifyMonitoringReportResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyMonitoringReportResponse1 is null || NotifyMonitoringReportResponse2 is null)
                return false;

            return NotifyMonitoringReportResponse1.Equals(NotifyMonitoringReportResponse2);

        }

        #endregion

        #region Operator != (NotifyMonitoringReportResponse1, NotifyMonitoringReportResponse2)

        /// <summary>
        /// Compares two NotifyMonitoringReport responses for inequality.
        /// </summary>
        /// <param name="NotifyMonitoringReportResponse1">A NotifyMonitoringReport response.</param>
        /// <param name="NotifyMonitoringReportResponse2">Another NotifyMonitoringReport response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyMonitoringReportResponse? NotifyMonitoringReportResponse1,
                                           NotifyMonitoringReportResponse? NotifyMonitoringReportResponse2)

            => !(NotifyMonitoringReportResponse1 == NotifyMonitoringReportResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyMonitoringReportResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyMonitoringReport responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyMonitoringReport response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyMonitoringReportResponse notifyMonitoringReportResponse &&
                   Equals(notifyMonitoringReportResponse);

        #endregion

        #region Equals(NotifyMonitoringReportResponse)

        /// <summary>
        /// Compares two NotifyMonitoringReport responses for equality.
        /// </summary>
        /// <param name="NotifyMonitoringReportResponse">A NotifyMonitoringReport response to compare with.</param>
        public override Boolean Equals(NotifyMonitoringReportResponse? NotifyMonitoringReportResponse)

            => NotifyMonitoringReportResponse is not null &&
                   base.GenericEquals(NotifyMonitoringReportResponse);

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

            => "NotifyMonitoringReportResponse";

        #endregion

    }

}
