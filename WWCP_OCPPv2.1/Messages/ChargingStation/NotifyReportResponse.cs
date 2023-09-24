/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A notify report response.
    /// </summary>
    public class NotifyReportResponse : AResponse<CS.NotifyReportRequest,
                                                  NotifyReportResponse>
    {

        #region Constructor(s)

        #region NotifyReportResponse(Request, ...)

        /// <summary>
        /// Create a new notify report response.
        /// </summary>
        /// <param name="Request">The notify report request leading to this response.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public NotifyReportResponse(CS.NotifyReportRequest   Request,

                                    IEnumerable<Signature>?  Signatures   = null,
                                    CustomData?              CustomData   = null)

            : base(Request,
                   Result.OK(),
                   Signatures,
                   CustomData)

        { }

        #endregion

        #region NotifyReportResponse(Request, Result)

        /// <summary>
        /// Create a new notify report response.
        /// </summary>
        /// <param name="Request">The notify report request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyReportResponse(CS.NotifyReportRequest  Request,
                                    Result                  Result)

            : base(Request,
                   Result,
                   Timestamp.Now)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyReportResponse",
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

        #region (static) Parse   (Request, JSON, CustomNotifyReportResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify report response.
        /// </summary>
        /// <param name="Request">The notify report request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyReportResponseParser">A delegate to parse custom notify report responses.</param>
        public static NotifyReportResponse Parse(CS.NotifyReportRequest                              Request,
                                                 JObject                                             JSON,
                                                 CustomJObjectParserDelegate<NotifyReportResponse>?  CustomNotifyReportResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var notifyReportResponse,
                         out var errorResponse,
                         CustomNotifyReportResponseParser))
            {
                return notifyReportResponse!;
            }

            throw new ArgumentException("The given JSON representation of a notify report response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyReportResponse, out ErrorResponse, CustomNotifyReportResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify report response.
        /// </summary>
        /// <param name="Request">The notify report request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyReportResponse">The parsed notify report response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyReportResponseParser">A delegate to parse custom notify report responses.</param>
        public static Boolean TryParse(CS.NotifyReportRequest                              Request,
                                       JObject                                             JSON,
                                       out NotifyReportResponse?                           NotifyReportResponse,
                                       out String?                                         ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyReportResponse>?  CustomNotifyReportResponseParser   = null)
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyReportResponse = new NotifyReportResponse(
                                           Request,
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
                ErrorResponse         = "The given JSON representation of a notify report response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyReportResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyReportResponseSerializer">A delegate to serialize custom notify report responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyReportResponse>?  CustomNotifyReportResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
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

            return CustomNotifyReportResponseSerializer is not null
                       ? CustomNotifyReportResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The notify report request failed.
        /// </summary>
        public static NotifyReportResponse Failed(CS.NotifyReportRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (NotifyReportResponse1, NotifyReportResponse2)

        /// <summary>
        /// Compares two notify report responses for equality.
        /// </summary>
        /// <param name="NotifyReportResponse1">A notify report response.</param>
        /// <param name="NotifyReportResponse2">Another notify report response.</param>
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
        /// Compares two notify report responses for inequality.
        /// </summary>
        /// <param name="NotifyReportResponse1">A notify report response.</param>
        /// <param name="NotifyReportResponse2">Another notify report response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyReportResponse? NotifyReportResponse1,
                                           NotifyReportResponse? NotifyReportResponse2)

            => !(NotifyReportResponse1 == NotifyReportResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyReportResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify report responses for equality.
        /// </summary>
        /// <param name="Object">A notify report response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyReportResponse notifyReportResponse &&
                   Equals(notifyReportResponse);

        #endregion

        #region Equals(NotifyReportResponse)

        /// <summary>
        /// Compares two notify report responses for equality.
        /// </summary>
        /// <param name="NotifyReportResponse">A notify report response to compare with.</param>
        public override Boolean Equals(NotifyReportResponse? NotifyReportResponse)

            => NotifyReportResponse is not null &&
                   base.GenericEquals(NotifyReportResponse);

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

            => "NotifyReportResponse";

        #endregion

    }

}
