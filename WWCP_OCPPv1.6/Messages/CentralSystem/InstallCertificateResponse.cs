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
    /// An install certificate response.
    /// </summary>
    public class InstallCertificateResponse : AResponse<CS.InstallCertificateRequest,
                                                           InstallCertificateResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the install certificate request.
        /// </summary>
        public CertificateStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region InstallCertificateResponse(Request, Status)

        /// <summary>
        /// Create a new install certificate response.
        /// </summary>
        /// <param name="Request">The install certificate request leading to this response.</param>
        /// <param name="Status">The success or failure of the install certificate request.</param>
        public InstallCertificateResponse(CS.InstallCertificateRequest  Request,
                                          CertificateStatus             Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region InstallCertificateResponse(Request, Result)

        /// <summary>
        /// Create a new install certificate response.
        /// </summary>
        /// <param name="Request">The install certificate request leading to this response.</param>
        /// <param name="Result">A result.</param>
        public InstallCertificateResponse(CS.InstallCertificateRequest  Request,
                                          Result                        Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:1.6:2020:3:InstallCertificate.conf",
        //   "definitions": {
        //     "InstallCertificateStatusEnumType": {
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Failed",
        //         "Rejected"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "status": {
        //       "$ref": "#/definitions/InstallCertificateStatusEnumType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomInstallCertificateResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an install certificate response.
        /// </summary>
        /// <param name="Request">The install certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomInstallCertificateResponseParser">A delegate to parse custom install certificate responses.</param>
        public static InstallCertificateResponse Parse(CS.InstallCertificateRequest                              Request,
                                                       JObject                                                   JSON,
                                                       CustomJObjectParserDelegate<InstallCertificateResponse>?  CustomInstallCertificateResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var installCertificateResponse,
                         out var errorResponse,
                         CustomInstallCertificateResponseParser))
            {
                return installCertificateResponse!;
            }

            throw new ArgumentException("The given JSON representation of an install certificate response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out InstallCertificateResponse, out ErrorResponse, CustomInstallCertificateResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an install certificate response.
        /// </summary>
        /// <param name="Request">The install certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="InstallCertificateResponse">The parsed install certificate response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomInstallCertificateResponseParser">A delegate to parse custom install certificate responses.</param>
        public static Boolean TryParse(CS.InstallCertificateRequest                              Request,
                                       JObject                                                   JSON,
                                       out InstallCertificateResponse?                           InstallCertificateResponse,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<InstallCertificateResponse>?  CustomInstallCertificateResponseParser   = null)
        {

            try
            {

                InstallCertificateResponse = null;

                #region Status

                if (!JSON.MapMandatory("status",
                                       "install certificate status",
                                       CertificateStatusExtensions.Parse,
                                       out CertificateStatus Status,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion


                InstallCertificateResponse = new InstallCertificateResponse(Request,
                                                                            Status);

                if (CustomInstallCertificateResponseParser is not null)
                    InstallCertificateResponse = CustomInstallCertificateResponseParser(JSON,
                                                                                        InstallCertificateResponse);

                return true;

            }
            catch (Exception e)
            {
                InstallCertificateResponse  = null;
                ErrorResponse               = "The given JSON representation of an install certificate response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomInstallCertificateResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomInstallCertificateResponseSerializer">A delegate to serialize custom install certificate responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<InstallCertificateResponse>? CustomInstallCertificateResponseSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomInstallCertificateResponseSerializer is not null
                       ? CustomInstallCertificateResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The install certificate failed.
        /// </summary>
        /// <param name="Request">The install certificate request leading to this response.</param>
        public static InstallCertificateResponse Failed(CS.InstallCertificateRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (InstallCertificateResponse1, InstallCertificateResponse2)

        /// <summary>
        /// Compares two install certificate responses for equality.
        /// </summary>
        /// <param name="InstallCertificateResponse1">An install certificate response.</param>
        /// <param name="InstallCertificateResponse2">Another install certificate response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (InstallCertificateResponse? InstallCertificateResponse1,
                                           InstallCertificateResponse? InstallCertificateResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(InstallCertificateResponse1, InstallCertificateResponse2))
                return true;

            // If one is null, but not both, return false.
            if (InstallCertificateResponse1 is null || InstallCertificateResponse2 is null)
                return false;

            return InstallCertificateResponse1.Equals(InstallCertificateResponse2);

        }

        #endregion

        #region Operator != (InstallCertificateResponse1, InstallCertificateResponse2)

        /// <summary>
        /// Compares two install certificate responses for inequality.
        /// </summary>
        /// <param name="InstallCertificateResponse1">An install certificate response.</param>
        /// <param name="InstallCertificateResponse2">Another install certificate response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (InstallCertificateResponse? InstallCertificateResponse1,
                                           InstallCertificateResponse? InstallCertificateResponse2)

            => !(InstallCertificateResponse1 == InstallCertificateResponse2);

        #endregion

        #endregion

        #region IEquatable<InstallCertificateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two install certificate responses for equality.
        /// </summary>
        /// <param name="Object">An install certificate response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is InstallCertificateResponse installCertificateResponse &&
                   Equals(installCertificateResponse);

        #endregion

        #region Equals(InstallCertificateResponse)

        /// <summary>
        /// Compares two install certificate responses for equality.
        /// </summary>
        /// <param name="InstallCertificateResponse">An install certificate response to compare with.</param>
        public override Boolean Equals(InstallCertificateResponse? InstallCertificateResponse)

            => InstallCertificateResponse is not null &&
                   Status.Equals(InstallCertificateResponse.Status);

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
