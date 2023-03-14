﻿/*
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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A signed update firmware response.
    /// </summary>
    public class SignedUpdateFirmwareResponse : AResponse<CS.SignedUpdateFirmwareRequest,
                                                             SignedUpdateFirmwareResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the signed update firmware request.
        /// </summary>
        public UpdateFirmwareStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region SignedUpdateFirmwareResponse(Request, Status)

        /// <summary>
        /// Create a new signed update firmware response.
        /// </summary>
        /// <param name="Request">The signed update firmware request leading to this response.</param>
        /// <param name="Status">The success or failure of the signed update firmware request.</param>
        public SignedUpdateFirmwareResponse(CS.SignedUpdateFirmwareRequest  Request,
                                            UpdateFirmwareStatus            Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region SignedUpdateFirmwareResponse(Request, Result)

        /// <summary>
        /// Create a new signed update firmware response.
        /// </summary>
        /// <param name="Request">The signed update firmware request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public SignedUpdateFirmwareResponse(CS.SignedUpdateFirmwareRequest  Request,
                                            Result                          Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:SignedUpdateFirmware.conf",
        //   "definitions": {
        //     "UpdateFirmwareStatusEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "AcceptedCanceled",
        //         "InvalidCertificate",
        //         "RevokedCertificate"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "status": {
        //       "$ref": "#/definitions/UpdateFirmwareStatusEnumType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomSignedUpdateFirmwareResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a signed update firmware response.
        /// </summary>
        /// <param name="Request">The signed update firmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSignedUpdateFirmwareResponseParser">A delegate to parse custom signed update firmware responses.</param>
        public static SignedUpdateFirmwareResponse Parse(CS.SignedUpdateFirmwareRequest                              Request,
                                                         JObject                                                     JSON,
                                                         CustomJObjectParserDelegate<SignedUpdateFirmwareResponse>?  CustomSignedUpdateFirmwareResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var signedUpdateFirmwareResponse,
                         out var errorResponse,
                         CustomSignedUpdateFirmwareResponseParser))
            {
                return signedUpdateFirmwareResponse!;
            }

            throw new ArgumentException("The given JSON representation of a signed update firmware response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SignedUpdateFirmwareResponse, out ErrorResponse, CustomSignedUpdateFirmwareResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a signed update firmware response.
        /// </summary>
        /// <param name="Request">The signed update firmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SignedUpdateFirmwareResponse">The parsed signed update firmware response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignedUpdateFirmwareResponseParser">A delegate to parse custom signed update firmware responses.</param>
        public static Boolean TryParse(CS.SignedUpdateFirmwareRequest                              Request,
                                       JObject                                                     JSON,
                                       out SignedUpdateFirmwareResponse?                           SignedUpdateFirmwareResponse,
                                       out String?                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<SignedUpdateFirmwareResponse>?  CustomSignedUpdateFirmwareResponseParser   = null)
        {

            try
            {

                SignedUpdateFirmwareResponse = null;

                #region Status

                if (!JSON.MapMandatory("status",
                                       "signed update firmware status",
                                       UpdateFirmwareStatusExtensions.Parse,
                                       out UpdateFirmwareStatus Status,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                SignedUpdateFirmwareResponse = new SignedUpdateFirmwareResponse(Request,
                                                                                Status);


                if (CustomSignedUpdateFirmwareResponseParser is not null)
                    SignedUpdateFirmwareResponse = CustomSignedUpdateFirmwareResponseParser(JSON,
                                                                                SignedUpdateFirmwareResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SignedUpdateFirmwareResponse  = null;
                ErrorResponse                 = "The given JSON representation of a signed update firmware response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSignedUpdateFirmwareResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignedUpdateFirmwareResponseSerializer">A delegate to serialize custom signed update firmware responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SignedUpdateFirmwareResponse>? CustomSignedUpdateFirmwareResponseSerializer = null)
        {

            var json = JSONObject.Create();

            return CustomSignedUpdateFirmwareResponseSerializer is not null
                       ? CustomSignedUpdateFirmwareResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The signed update firmware command failed.
        /// </summary>
        /// <param name="Request">The signed update firmware request leading to this response.</param>
        public static SignedUpdateFirmwareResponse Failed(CS.SignedUpdateFirmwareRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (SignedUpdateFirmwareResponse1, SignedUpdateFirmwareResponse2)

        /// <summary>
        /// Compares two signed update firmware responses for equality.
        /// </summary>
        /// <param name="SignedUpdateFirmwareResponse1">A signed update firmware response.</param>
        /// <param name="SignedUpdateFirmwareResponse2">Another signed update firmware response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SignedUpdateFirmwareResponse? SignedUpdateFirmwareResponse1,
                                           SignedUpdateFirmwareResponse? SignedUpdateFirmwareResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignedUpdateFirmwareResponse1, SignedUpdateFirmwareResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SignedUpdateFirmwareResponse1 is null || SignedUpdateFirmwareResponse2 is null)
                return false;

            return SignedUpdateFirmwareResponse1.Equals(SignedUpdateFirmwareResponse2);

        }

        #endregion

        #region Operator != (SignedUpdateFirmwareResponse1, SignedUpdateFirmwareResponse2)

        /// <summary>
        /// Compares two signed update firmware responses for inequality.
        /// </summary>
        /// <param name="SignedUpdateFirmwareResponse1">A signed update firmware response.</param>
        /// <param name="SignedUpdateFirmwareResponse2">Another signed update firmware response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SignedUpdateFirmwareResponse? SignedUpdateFirmwareResponse1,
                                           SignedUpdateFirmwareResponse? SignedUpdateFirmwareResponse2)

            => !(SignedUpdateFirmwareResponse1 == SignedUpdateFirmwareResponse2);

        #endregion

        #endregion

        #region IEquatable<SignedUpdateFirmwareResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signed update firmware responses for equality.
        /// </summary>
        /// <param name="Object">A signed update firmware response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignedUpdateFirmwareResponse signedUpdateFirmwareResponse &&
                   Equals(signedUpdateFirmwareResponse);

        #endregion

        #region Equals(SignedUpdateFirmwareResponse)

        /// <summary>
        /// Compares two signed update firmware responses for equality.
        /// </summary>
        /// <param name="SignedUpdateFirmwareResponse">A signed update firmware response to compare with.</param>
        public override Boolean Equals(SignedUpdateFirmwareResponse? SignedUpdateFirmwareResponse)

            => SignedUpdateFirmwareResponse is not null &&
                   Status.Equals(SignedUpdateFirmwareResponse.Status);

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
