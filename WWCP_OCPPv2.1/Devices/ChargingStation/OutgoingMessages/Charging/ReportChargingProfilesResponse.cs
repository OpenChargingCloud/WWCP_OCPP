﻿/*
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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The ReportChargingProfiles response.
    /// </summary>
    public class ReportChargingProfilesResponse : AResponse<CS.ReportChargingProfilesRequest,
                                                            ReportChargingProfilesResponse>,
                                                  IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/reportChargingProfilesResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        #region ReportChargingProfilesResponse(Request, ...)

        /// <summary>
        /// Create a new ReportChargingProfiles response.
        /// </summary>
        /// <param name="Request">The ReportChargingProfiles request leading to this response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ReportChargingProfilesResponse(CS.ReportChargingProfilesRequest  Request,
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

        { }

        #endregion

        #region ReportChargingProfilesResponse(Request, Result)

        /// <summary>
        /// Create a new ReportChargingProfiles response.
        /// </summary>
        /// <param name="Request">The ReportChargingProfiles request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ReportChargingProfilesResponse(CS.ReportChargingProfilesRequest  Request,
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
        //   "$id": "urn:OCPP:Cp:2:2020:3:ReportChargingProfilesResponse",
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

        #region (static) Parse   (Request, JSON, CustomReportChargingProfilesResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ReportChargingProfiles response.
        /// </summary>
        /// <param name="Request">The ReportChargingProfiles request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomReportChargingProfilesResponseParser">A delegate to parse custom ReportChargingProfiles responses.</param>
        public static ReportChargingProfilesResponse Parse(CS.ReportChargingProfilesRequest                              Request,
                                                           JObject                                                       JSON,
                                                           CustomJObjectParserDelegate<ReportChargingProfilesResponse>?  CustomReportChargingProfilesResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var reportChargingProfilesResponse,
                         out var errorResponse,
                         CustomReportChargingProfilesResponseParser))
            {
                return reportChargingProfilesResponse;
            }

            throw new ArgumentException("The given JSON representation of a ReportChargingProfiles response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ReportChargingProfilesResponse, out ErrorResponse, CustomReportChargingProfilesResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ReportChargingProfiles response.
        /// </summary>
        /// <param name="Request">The ReportChargingProfiles request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ReportChargingProfilesResponse">The parsed ReportChargingProfiles response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomReportChargingProfilesResponseParser">A delegate to parse custom ReportChargingProfiles responses.</param>
        public static Boolean TryParse(CS.ReportChargingProfilesRequest                              Request,
                                       JObject                                                       JSON,
                                       [NotNullWhen(true)]  out ReportChargingProfilesResponse?      ReportChargingProfilesResponse,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       CustomJObjectParserDelegate<ReportChargingProfilesResponse>?  CustomReportChargingProfilesResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                ReportChargingProfilesResponse = null;

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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ReportChargingProfilesResponse = new ReportChargingProfilesResponse(
                                                     Request,
                                                     null,
                                                     null,
                                                     null,
                                                     Signatures,
                                                     CustomData
                                                 );

                if (CustomReportChargingProfilesResponseParser is not null)
                    ReportChargingProfilesResponse = CustomReportChargingProfilesResponseParser(JSON,
                                                                                                ReportChargingProfilesResponse);

                return true;

            }
            catch (Exception e)
            {
                ReportChargingProfilesResponse  = null;
                ErrorResponse                   = "The given JSON representation of a ReportChargingProfiles response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomReportChargingProfilesResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReportChargingProfilesResponseSerializer">A delegate to serialize custom ReportChargingProfiles responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ReportChargingProfilesResponse>?  CustomReportChargingProfilesResponseSerializer   = null,
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

            return CustomReportChargingProfilesResponseSerializer is not null
                       ? CustomReportChargingProfilesResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ReportChargingProfiles failed because of a request error.
        /// </summary>
        /// <param name="Request">The ReportChargingProfiles request.</param>
        public static ReportChargingProfilesResponse RequestError(CS.ReportChargingProfilesRequest  Request,
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
        /// The ReportChargingProfiles failed.
        /// </summary>
        /// <param name="Request">The ReportChargingProfiles request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ReportChargingProfilesResponse SignatureError(CS.ReportChargingProfilesRequest  Request,
                                                                    String                            ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The ReportChargingProfiles failed.
        /// </summary>
        /// <param name="Request">The ReportChargingProfiles request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ReportChargingProfilesResponse Failed(CS.ReportChargingProfilesRequest  Request,
                                                            String?                           Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The ReportChargingProfiles failed because of an exception.
        /// </summary>
        /// <param name="Request">The ReportChargingProfiles request.</param>
        /// <param name="Exception">The exception.</param>
        public static ReportChargingProfilesResponse ExceptionOccured(CS.ReportChargingProfilesRequest  Request,
                                                                      Exception                         Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ReportChargingProfilesResponse1, ReportChargingProfilesResponse2)

        /// <summary>
        /// Compares two ReportChargingProfiles responses for equality.
        /// </summary>
        /// <param name="ReportChargingProfilesResponse1">A ReportChargingProfiles response.</param>
        /// <param name="ReportChargingProfilesResponse2">Another ReportChargingProfiles response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ReportChargingProfilesResponse? ReportChargingProfilesResponse1,
                                           ReportChargingProfilesResponse? ReportChargingProfilesResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ReportChargingProfilesResponse1, ReportChargingProfilesResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ReportChargingProfilesResponse1 is null || ReportChargingProfilesResponse2 is null)
                return false;

            return ReportChargingProfilesResponse1.Equals(ReportChargingProfilesResponse2);

        }

        #endregion

        #region Operator != (ReportChargingProfilesResponse1, ReportChargingProfilesResponse2)

        /// <summary>
        /// Compares two ReportChargingProfiles responses for inequality.
        /// </summary>
        /// <param name="ReportChargingProfilesResponse1">A ReportChargingProfiles response.</param>
        /// <param name="ReportChargingProfilesResponse2">Another ReportChargingProfiles response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ReportChargingProfilesResponse? ReportChargingProfilesResponse1,
                                           ReportChargingProfilesResponse? ReportChargingProfilesResponse2)

            => !(ReportChargingProfilesResponse1 == ReportChargingProfilesResponse2);

        #endregion

        #endregion

        #region IEquatable<ReportChargingProfilesResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ReportChargingProfiles responses for equality.
        /// </summary>
        /// <param name="Object">A ReportChargingProfiles response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReportChargingProfilesResponse reportChargingProfilesResponse &&
                   Equals(reportChargingProfilesResponse);

        #endregion

        #region Equals(ReportChargingProfilesResponse)

        /// <summary>
        /// Compares two ReportChargingProfiles responses for equality.
        /// </summary>
        /// <param name="ReportChargingProfilesResponse">A ReportChargingProfiles response to compare with.</param>
        public override Boolean Equals(ReportChargingProfilesResponse? ReportChargingProfilesResponse)

            => ReportChargingProfilesResponse is not null &&
                   base.GenericEquals(ReportChargingProfilesResponse);

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

            => "ReportChargingProfilesResponse";

        #endregion

    }

}
