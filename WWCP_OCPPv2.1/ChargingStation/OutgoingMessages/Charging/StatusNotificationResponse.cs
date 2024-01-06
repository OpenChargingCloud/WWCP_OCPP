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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A status notification response.
    /// </summary>
    public class StatusNotificationResponse : AResponse<CS.StatusNotificationRequest,
                                                        StatusNotificationResponse>,
                                              IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/statusNotificationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        #region StatusNotificationResponse(Request, ...)

        /// <summary>
        /// Create a new status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public StatusNotificationResponse(CS.StatusNotificationRequest  Request,
                                          DateTime?                     ResponseTimestamp   = null,

                                          IEnumerable<KeyPair>?         SignKeys            = null,
                                          IEnumerable<SignInfo>?        SignInfos           = null,
                                          IEnumerable<OCPP.Signature>?  Signatures          = null,

                                          CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #region StatusNotificationResponse(Request, Result)

        /// <summary>
        /// Create a new status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public StatusNotificationResponse(CS.StatusNotificationRequest  Request,
                                          Result                        Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:StatusNotificationResponse",
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

        #region (static) Parse   (Request, JSON, CustomStatusNotificationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomStatusNotificationResponseParser">A delegate to parse custom status notification responses.</param>
        public static StatusNotificationResponse Parse(CS.StatusNotificationRequest                              Request,
                                                       JObject                                                   JSON,
                                                       CustomJObjectParserDelegate<StatusNotificationResponse>?  CustomStatusNotificationResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var statusNotificationResponse,
                         out var errorResponse,
                         CustomStatusNotificationResponseParser))
            {
                return statusNotificationResponse;
            }

            throw new ArgumentException("The given JSON representation of a status notification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out StatusNotificationResponse, out ErrorResponse, CustomStatusNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="StatusNotificationResponse">The parsed status notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStatusNotificationResponseParser">A delegate to parse custom status notification responses.</param>
        public static Boolean TryParse(CS.StatusNotificationRequest                              Request,
                                       JObject                                                   JSON,
                                       [NotNullWhen(true)]  out StatusNotificationResponse?      StatusNotificationResponse,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       CustomJObjectParserDelegate<StatusNotificationResponse>?  CustomStatusNotificationResponseParser   = null)
        {

            try
            {

                StatusNotificationResponse = null;

                #region Signatures    [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                StatusNotificationResponse = new StatusNotificationResponse(
                                                 Request,
                                                 null,
                                                 null,
                                                 null,
                                                 Signatures,
                                                 CustomData
                                             );

                if (CustomStatusNotificationResponseParser is not null)
                    StatusNotificationResponse = CustomStatusNotificationResponseParser(JSON,
                                                                                        StatusNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                StatusNotificationResponse  = null;
                ErrorResponse               = "The given JSON representation of a status notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomStatusNotificationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStatusNotificationResponseSerializer">A delegate to serialize custom status notification responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StatusNotificationResponse>?  CustomStatusNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?              CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
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

            return CustomStatusNotificationResponseSerializer is not null
                       ? CustomStatusNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The status notification failed.
        /// </summary>
        public static StatusNotificationResponse Failed(CS.StatusNotificationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (StatusNotificationResponse1, StatusNotificationResponse2)

        /// <summary>
        /// Compares two status notification responses for equality.
        /// </summary>
        /// <param name="StatusNotificationResponse1">A status notification response.</param>
        /// <param name="StatusNotificationResponse2">Another status notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StatusNotificationResponse? StatusNotificationResponse1,
                                           StatusNotificationResponse? StatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StatusNotificationResponse1, StatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (StatusNotificationResponse1 is null || StatusNotificationResponse2 is null)
                return false;

            return StatusNotificationResponse1.Equals(StatusNotificationResponse2);

        }

        #endregion

        #region Operator != (StatusNotificationResponse1, StatusNotificationResponse2)

        /// <summary>
        /// Compares two status notification responses for inequality.
        /// </summary>
        /// <param name="StatusNotificationResponse1">A status notification response.</param>
        /// <param name="StatusNotificationResponse2">Another status notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StatusNotificationResponse? StatusNotificationResponse1,
                                           StatusNotificationResponse? StatusNotificationResponse2)

            => !(StatusNotificationResponse1 == StatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<StatusNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two status notification responses for equality.
        /// </summary>
        /// <param name="Object">A status notification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StatusNotificationResponse statusNotificationResponse &&
                   Equals(statusNotificationResponse);

        #endregion

        #region Equals(StatusNotificationResponse)

        /// <summary>
        /// Compares two status notification responses for equality.
        /// </summary>
        /// <param name="StatusNotificationResponse">A status notification response to compare with.</param>
        public override Boolean Equals(StatusNotificationResponse? StatusNotificationResponse)

            => StatusNotificationResponse is not null &&

             ((CustomData is     null && StatusNotificationResponse.CustomData is     null) ||
              (CustomData is not null && StatusNotificationResponse.CustomData is not null && CustomData.Equals(StatusNotificationResponse.CustomData)));

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

                return base.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "StatusNotificationResponse";

        #endregion

    }

}
