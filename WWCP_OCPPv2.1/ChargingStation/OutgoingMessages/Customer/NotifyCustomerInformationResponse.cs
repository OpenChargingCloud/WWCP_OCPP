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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A notify customer information response.
    /// </summary>
    public class NotifyCustomerInformationResponse : AResponse<CS.NotifyCustomerInformationRequest,
                                                               NotifyCustomerInformationResponse>,
                                                     IResponse
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

        #region NotifyCustomerInformationResponse(Request, ...)

        /// <summary>
        /// Create a new notify customer information response.
        /// </summary>
        /// <param name="Request">The notify customer information request leading to this response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public NotifyCustomerInformationResponse(CS.NotifyCustomerInformationRequest  Request,
                                                 DateTime?                            ResponseTimestamp   = null,

                                                 IEnumerable<KeyPair>?                SignKeys            = null,
                                                 IEnumerable<SignInfo>?               SignInfos           = null,
                                                 IEnumerable<OCPP.Signature>?         Signatures          = null,

                                                 CustomData?                          CustomData          = null)

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

        #region NotifyCustomerInformationResponse(Request, Result)

        /// <summary>
        /// Create a new notify customer information response.
        /// </summary>
        /// <param name="Request">The notify customer information request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyCustomerInformationResponse(CS.NotifyCustomerInformationRequest  Request,
                                                 Result                               Result)

            : base(Request,
                   Result)

        { }

        #endregion

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
        /// Parse the given JSON representation of a notify customer information response.
        /// </summary>
        /// <param name="Request">The notify customer information request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyCustomerInformationResponseParser">A delegate to parse custom notify customer information responses.</param>
        public static NotifyCustomerInformationResponse Parse(CS.NotifyCustomerInformationRequest                              Request,
                                                              JObject                                                          JSON,
                                                              CustomJObjectParserDelegate<NotifyCustomerInformationResponse>?  CustomNotifyCustomerInformationResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var notifyCustomerInformationResponse,
                         out var errorResponse,
                         CustomNotifyCustomerInformationResponseParser))
            {
                return notifyCustomerInformationResponse;
            }

            throw new ArgumentException("The given JSON representation of a notify customer information response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyCustomerInformationResponse, out ErrorResponse, CustomNotifyCustomerInformationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify customer information response.
        /// </summary>
        /// <param name="Request">The notify customer information request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyCustomerInformationResponse">The parsed notify customer information response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyCustomerInformationResponseParser">A delegate to parse custom notify customer information responses.</param>
        public static Boolean TryParse(CS.NotifyCustomerInformationRequest                              Request,
                                       JObject                                                          JSON,
                                       [NotNullWhen(true)]  out NotifyCustomerInformationResponse?      NotifyCustomerInformationResponse,
                                       [NotNullWhen(false)] out String?                                 ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyCustomerInformationResponse>?  CustomNotifyCustomerInformationResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                NotifyCustomerInformationResponse = null;

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
                ErrorResponse                      = "The given JSON representation of a notify customer information response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyCustomerInformationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyCustomerInformationResponseSerializer">A delegate to serialize custom notify customer information responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyCustomerInformationResponse>?  CustomNotifyCustomerInformationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                     CustomSignatureSerializer                           = null,
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
        /// The notify customer information request failed.
        /// </summary>
        public static NotifyCustomerInformationResponse Failed(CS.NotifyCustomerInformationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (NotifyCustomerInformationResponse1, NotifyCustomerInformationResponse2)

        /// <summary>
        /// Compares two notify customer information responses for equality.
        /// </summary>
        /// <param name="NotifyCustomerInformationResponse1">A notify customer information response.</param>
        /// <param name="NotifyCustomerInformationResponse2">Another notify customer information response.</param>
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
        /// Compares two notify customer information responses for inequality.
        /// </summary>
        /// <param name="NotifyCustomerInformationResponse1">A notify customer information response.</param>
        /// <param name="NotifyCustomerInformationResponse2">Another notify customer information response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyCustomerInformationResponse? NotifyCustomerInformationResponse1,
                                           NotifyCustomerInformationResponse? NotifyCustomerInformationResponse2)

            => !(NotifyCustomerInformationResponse1 == NotifyCustomerInformationResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyCustomerInformationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify customer information responses for equality.
        /// </summary>
        /// <param name="Object">A notify customer information response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyCustomerInformationResponse notifyCustomerInformationResponse &&
                   Equals(notifyCustomerInformationResponse);

        #endregion

        #region Equals(NotifyCustomerInformationResponse)

        /// <summary>
        /// Compares two notify customer information responses for equality.
        /// </summary>
        /// <param name="NotifyCustomerInformationResponse">A notify customer information response to compare with.</param>
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
