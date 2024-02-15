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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A delete certificate response.
    /// </summary>
    [SecurityExtensions]
    public class DeleteCertificateResponse : AResponse<CS.DeleteCertificateRequest,
                                                          DeleteCertificateResponse>,
                                             IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/deleteCertificateResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext            Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the delete certificate request.
        /// </summary>
        [Mandatory]
        public DeleteCertificateStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region DeleteCertificateResponse(Request, Status)

        /// <summary>
        /// Create a new delete certificate response.
        /// </summary>
        /// <param name="Request">The delete certificate request leading to this response.</param>
        /// <param name="Status">The success or failure of the delete certificate request.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public DeleteCertificateResponse(CS.DeleteCertificateRequest   Request,
                                         DeleteCertificateStatus       Status,

                                         DateTime?                     ResponseTimestamp   = null,

                                         IEnumerable<KeyPair>?         SignKeys            = null,
                                         IEnumerable<SignInfo>?        SignInfos           = null,
                                         IEnumerable<OCPP.Signature>?  Signatures          = null,

                                         CustomData?                   CustomData          = null)

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

            this.Status = Status;

        }

        #endregion

        #region DeleteCertificateResponse(Request, Result)

        /// <summary>
        /// Create a new delete certificate response.
        /// </summary>
        /// <param name="Request">The delete certificate request leading to this response.</param>
        /// <param name="Result">A result.</param>
        public DeleteCertificateResponse(CS.DeleteCertificateRequest  Request,
                                         Result                       Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:DeleteCertificate.conf",
        //   "definitions": {
        //     "DeleteCertificateStatusEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Failed",
        //         "NotFound"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "status": {
        //       "$ref": "#/definitions/DeleteCertificateStatusEnumType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomDeleteCertificateResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a delete certificate response.
        /// </summary>
        /// <param name="Request">The delete certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDeleteCertificateResponseParser">An optional delegate to parse custom delete certificate responses.</param>
        public static DeleteCertificateResponse Parse(CS.DeleteCertificateRequest                              Request,
                                                      JObject                                                  JSON,
                                                      CustomJObjectParserDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var deleteCertificateResponse,
                         out var errorResponse,
                         CustomDeleteCertificateResponseParser) &&
                deleteCertificateResponse is not null)
            {
                return deleteCertificateResponse;
            }

            throw new ArgumentException("The given JSON representation of a delete certificate response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out DeleteCertificateResponse, out ErrorResponse, CustomDeleteCertificateResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a delete certificate response.
        /// </summary>
        /// <param name="Request">The delete certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DeleteCertificateResponse">The parsed delete certificate response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDeleteCertificateResponseParser">An optional delegate to parse custom delete certificate responses.</param>
        public static Boolean TryParse(CS.DeleteCertificateRequest                              Request,
                                       JObject                                                  JSON,
                                       out DeleteCertificateResponse?                           DeleteCertificateResponse,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseParser   = null)
        {

            try
            {

                DeleteCertificateResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "delete certificate status",
                                       DeleteCertificateStatusExtensions.Parse,
                                       out DeleteCertificateStatus Status,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

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


                DeleteCertificateResponse = new DeleteCertificateResponse(

                                                Request,
                                                Status,
                                                null,

                                                null,
                                                null,
                                                Signatures,

                                                CustomData

                                            );

                if (CustomDeleteCertificateResponseParser is not null)
                    DeleteCertificateResponse = CustomDeleteCertificateResponseParser(JSON,
                                                                                      DeleteCertificateResponse);

                return true;

            }
            catch (Exception e)
            {
                DeleteCertificateResponse  = null;
                ErrorResponse              = "The given JSON representation of a delete certificate response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDeleteCertificateResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteCertificateResponseSerializer">A delegate to serialize custom delete certificate responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?             CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
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

            return CustomDeleteCertificateResponseSerializer is not null
                       ? CustomDeleteCertificateResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The delete certificate failed.
        /// </summary>
        /// <param name="Request">The delete certificate request leading to this response.</param>
        public static DeleteCertificateResponse Failed(CS.DeleteCertificateRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (DeleteCertificateResponse1, DeleteCertificateResponse2)

        /// <summary>
        /// Compares two delete certificate responses for equality.
        /// </summary>
        /// <param name="DeleteCertificateResponse1">A delete certificate response.</param>
        /// <param name="DeleteCertificateResponse2">Another delete certificate response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DeleteCertificateResponse? DeleteCertificateResponse1,
                                           DeleteCertificateResponse? DeleteCertificateResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DeleteCertificateResponse1, DeleteCertificateResponse2))
                return true;

            // If one is null, but not both, return false.
            if (DeleteCertificateResponse1 is null || DeleteCertificateResponse2 is null)
                return false;

            return DeleteCertificateResponse1.Equals(DeleteCertificateResponse2);

        }

        #endregion

        #region Operator != (DeleteCertificateResponse1, DeleteCertificateResponse2)

        /// <summary>
        /// Compares two delete certificate responses for inequality.
        /// </summary>
        /// <param name="DeleteCertificateResponse1">A delete certificate response.</param>
        /// <param name="DeleteCertificateResponse2">Another delete certificate response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteCertificateResponse? DeleteCertificateResponse1,
                                           DeleteCertificateResponse? DeleteCertificateResponse2)

            => !(DeleteCertificateResponse1 == DeleteCertificateResponse2);

        #endregion

        #endregion

        #region IEquatable<DeleteCertificateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two delete certificate responses for equality.
        /// </summary>
        /// <param name="Object">A delete certificate response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteCertificateResponse deleteCertificateResponse &&
                   Equals(deleteCertificateResponse);

        #endregion

        #region Equals(DeleteCertificateResponse)

        /// <summary>
        /// Compares two delete certificate responses for equality.
        /// </summary>
        /// <param name="DeleteCertificateResponse">A delete certificate response to compare with.</param>
        public override Boolean Equals(DeleteCertificateResponse? DeleteCertificateResponse)

            => DeleteCertificateResponse is not null &&
                   Status.Equals(DeleteCertificateResponse.Status);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Status.GetHashCode();

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
